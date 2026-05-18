using Google.Apis.Auth;
using LoginServer.Models;
using LoginServer.Repositories;
using LoginServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServerCore.Service;
using SharedData.Request;
using SharedData.Response;
using SharedData.Type;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;

namespace Empen.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly IConfiguration _configuration;
        private readonly IRedisService _redisService;
        private readonly ITimeService _timeService;

        public AuthController(IPersonService personSerivce, IConfiguration configuration, IRedisService redisService, ITimeService timeService)
        {
            _personService = personSerivce;
            _configuration = configuration;
            _redisService = redisService;
            _timeService = timeService;
        }

        string LOGIN_PROVIDER_GOOGLE = "Google";

        // POST: auth/google-login
        [HttpPost("google-login")]
        public async Task<ActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            DateTime now = await _timeService.getNowAsync();

            try
            {
                // 구글 토큰 검증
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.idToken, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Google:ClientId"] }
                });

                string googleUserId = payload.Subject;
                string email = payload.Email;

                // DB에서 유저 조회 (없으면 회원가입)
                var person = await _personService.findByHashAsync(LOGIN_PROVIDER_GOOGLE, googleUserId);

                if (person == null)
                {
                    person = await _personService.registerPersonAsync(LOGIN_PROVIDER_GOOGLE, googleUserId, email);
                }

                // JWT 토큰 발급
                var tokens = await GenerateTokensAsync(person);

                // 토큰 반환
                return Ok(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized("Invalid Google Token");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        string LOGIN_PROVIDER_GUEST = "Guest";

        // POST: auth/guest-login
        [HttpPost("guest-login")]
        public async Task<ActionResult<GameResponse<GuestLoginResponse>>> GuestLogin([FromBody] GuestLoginRequest request)
        {
            DateTime now = await _timeService.getNowAsync();

            try
            {
                person? person = null;

                if (request.id != 0)
                {
                    person = await _personService.findByDisplayPersonIdAsync(LOGIN_PROVIDER_GUEST, request.id);
                }

                if (person == null)
                {
                    string guestHash = Guid.NewGuid().ToString();
                    person = await _personService.registerPersonAsync(LOGIN_PROVIDER_GUEST, guestHash, "Guest");
                }

                if (person == null)
                {
                    return Ok(new GameResponse<string>(ErrorCode.ServerError));
                }

                var tokens = await GenerateTokensAsync(person);

                GuestLoginResponse res = new GuestLoginResponse()
                {
                    accessToken = tokens.AccessToken,
                    refreshToken = tokens.RefreshToken,
                    displayPersonId = person.display_person_id
                };
                return Ok(new GameResponse<GuestLoginResponse>(res));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError));
            }
        }

        // POST: auth/refresh
        // 토큰 갱신 (Access Token 만료 시 호출)
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            // personId 추출
            var principal = GetPrincipalFromExpiredToken(request.accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid Access Token");
            }

            // claim에서 personId 확인
            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out int personId))
            {
                return BadRequest("Invalid Token Claims");
            }

            // 저장된 Refresh Token
            string key = $"RT:{personId}";
            string savedRefreshToken = await _redisService.getAsync<string>(key);

            // 검증
            if (string.IsNullOrEmpty(savedRefreshToken) || savedRefreshToken != request.refreshToken)
            {
                return Unauthorized("Invalid Refresh Token"); // 재로그인 필요
            }

            // 유저 정보 교차 확인
            //var person = await _personContext.person.FindAsync(personId);
            var person = await _personService.findByPersonIdAsync(personId);
            if (person == null)
            {
                return Unauthorized("User not found");
            }

            // 새 토큰 발급
            var newTokens = await GenerateTokensAsync(person);

            return Ok(new { accessToken = newTokens.AccessToken, refreshToken = newTokens.RefreshToken });
        }

        // 토큰 생성 및 Redis 저장 헬퍼
        private async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(person person)
        {
            // Access Token (JWT) 생성
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, person.person_id.ToString()),
                new Claim(ClaimTypes.Email, person.email ?? ""),
                new Claim("Hash", person.person_hash)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // 수명: 30분
                signingCredentials: credentials);

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Refresh Token 생성 (랜덤 문자열)
            string refreshToken = Guid.NewGuid().ToString();

            // Redis에 Refresh Token 저장 (수명 14일)
            // Key: "RT:{personId}", Value: "{refreshToken}"
            string key = $"RT:{person.person_id}";
            await _redisService.setAsync<string>(key, refreshToken, TimeSpan.FromDays(14));

            return (accessToken, refreshToken);
        }

        // 만료된 JWT에서 정보(Claims) 조회
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, // 여기선 검증 안함
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false // 만료되었어도 오류 안 나게 설정
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
