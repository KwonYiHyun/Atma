using Empen.Data;
using Empen.Service.IService;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerCore.Service;
using SharedData.Models;
using SharedData.Request;
using SharedData.Response;
using SharedData.Type;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Empen.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PersonDbContext _personContext;
        private readonly IConfiguration _configuration;
        private readonly IRedisService _redisService;
        private readonly ITimeService _timeService;
        private readonly IPersonService _personService;

        public AuthController(PersonDbContext personContext, IConfiguration configuration, IRedisService redisService, ITimeService timeService, IPersonService personSerivce)
        {
            _personContext = personContext;
            _configuration = configuration;
            _redisService = redisService;
            _timeService = timeService;
            _personService = personSerivce;
        }

        // POST: auth/create
        [HttpPost("create")]
        public async Task<ActionResult<GameResponse<string>>> createPerson([FromBody] CreatePersonRequest request)
        {
            var result = await _personService.createPerson(request.person_id, request.display_person_id, request.person_hash, request.email);

            if (result != ErrorCode.Success)
            {
                return Ok(new GameResponse<string>(result));
            }

            return Ok(new GameResponse<string>("성공"));
        }

        //// POST: auth/google-login
        //[HttpPost("google-login")]
        //public async Task<ActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        //{
        //    DateTime now = await _timeService.getNowAsync();

        //    try
        //    {
        //        // 1. 구글 토큰 검증
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(request.idToken, new GoogleJsonWebSignature.ValidationSettings()
        //        {
        //            // ClientId
        //            Audience = new List<string> { _configuration["Google:ClientId"] }
        //        });

        //        string googleUserId = payload.Subject;
        //        string email = payload.Email;

        //        // 2. DB에서 유저 조회 (없으면 회원가입)
        //        var person = await _personContext.person.FirstOrDefaultAsync(p => p.login_provider == Constant.LOGIN_PROVIDER_GOOGLE && p.person_hash == googleUserId);

        //        if (person == null)
        //        {
        //            // 트랜잭션 시작: person과 person_status가 모두 성공해야 함
        //            using (var transaction = await _personContext.Database.BeginTransactionAsync())
        //            {
        //                try
        //                {
        //                    // 2-1. 중복되지 않는 9자리 Display ID 생성
        //                    string newDisplayId = await GenerateUniqueDisplayIdAsync();

        //                    // 2-2. Person 생성
        //                    person = new person
        //                    {
        //                        person_hash = googleUserId,
        //                        login_provider = Constant.LOGIN_PROVIDER_GOOGLE,
        //                        email = email,
        //                        display_person_id = int.Parse(newDisplayId),
        //                        insert_date = now,
        //                        update_date = now
        //                    };

        //                    _personContext.person.Add(person);
        //                    // 여기서 SaveChanges를 해야 PK(person_id)가 생성되어 person 객체에 담김
        //                    await _personContext.SaveChangesAsync();

        //                    // 2-3. Person Status (기본 게임 데이터) 생성
        //                    var newStatus = new person_status
        //                    {
        //                        person_id = person.person_id,
        //                        person_name = Constant.DEFAULT_USERNAME,
        //                        level = 1,
        //                        exp = 0,
        //                        token = 1000,
        //                        gift = 0,
        //                        manual = 0,
        //                        flim = 99999,
        //                        prism = 0,
        //                        character_amount_max = 50,
        //                        character_storage_amount_max = 0,
        //                        leader_person_character_id = 0, // 아직 카드가 없음 (튜토리얼 후 설정 필요)
        //                        introduce = Constant.DEFAULT_INTRODUCE,
        //                        insert_date = now,
        //                        update_date = now
        //                    };

        //                    _personContext.person_status.Add(newStatus);
        //                    await _personContext.SaveChangesAsync();

        //                    // 모든 작업 성공 시 커밋
        //                    await transaction.CommitAsync();
        //                }
        //                catch (Exception)
        //                {
        //                    // 실패 시 롤백 (person만 생기는 것 방지)
        //                    await transaction.RollbackAsync();
        //                    throw; // 에러를 다시 던져서 500 응답으로 보냄
        //                }
        //            }
        //        }

        //        // 3. JWT 토큰 발급 (여기서 person_id를 심음)
        //        var tokens = await GenerateTokensAsync(person);

        //        // 4. 토큰 반환
        //        // return Ok(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken, nickname = person.nickname });
        //        return Ok(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });
        //    }
        //    catch (InvalidJwtException)
        //    {
        //        return Unauthorized("Invalid Google Token");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}

        //// POST: auth/google-login
        //[HttpPost("guest-login")]
        //public async Task<ActionResult<GameResponse<GuestLoginResponse>>> GuestLogin([FromBody] GuestLoginRequest request)
        //{
        //    DateTime now = await _timeService.getNowAsync();

        //    // 2. DB에서 유저 조회 (없으면 회원가입)
        //    var person = await _personContext.person.FirstOrDefaultAsync(p => p.login_provider == Constant.LOGIN_PROVIDER_GUEST && p.display_person_id == request.id);

        //    if (person == null)
        //    {
        //        // 트랜잭션 시작: person과 person_status가 모두 성공해야 함
        //        using (var transaction = await _personContext.Database.BeginTransactionAsync())
        //        {
        //            try
        //            {
        //                // 2-1. 중복되지 않는 9자리 Display ID 생성
        //                string newDisplayId = await GenerateUniqueDisplayIdAsync();

        //                // 2-2. Person 생성
        //                person = new person
        //                {
        //                    person_hash = "guest",
        //                    login_provider = Constant.LOGIN_PROVIDER_GUEST,
        //                    email = "guest",
        //                    display_person_id = int.Parse(newDisplayId),
        //                    insert_date = now,
        //                    update_date = now
        //                };

        //                _personContext.person.Add(person);
        //                // 여기서 SaveChanges를 해야 PK(person_id)가 생성되어 person 객체에 담김
        //                await _personContext.SaveChangesAsync();

        //                // 2-3. Person Status (기본 게임 데이터) 생성
        //                var newStatus = new person_status
        //                {
        //                    person_id = person.person_id,
        //                    person_name = Constant.DEFAULT_USERNAME,
        //                    level = 1,
        //                    exp = 0,
        //                    token = 1000,
        //                    gift = 0,
        //                    manual = 0,
        //                    flim = 99999,
        //                    prism = 0,
        //                    character_amount_max = 50,
        //                    character_storage_amount_max = 0,
        //                    leader_person_character_id = 0, // 아직 카드가 없음 (튜토리얼 후 설정 필요)
        //                    introduce = Constant.DEFAULT_INTRODUCE,
        //                    insert_date = now,
        //                    update_date = now
        //                };

        //                _personContext.person_status.Add(newStatus);
        //                await _personContext.SaveChangesAsync();

        //                // 모든 작업 성공 시 커밋
        //                await transaction.CommitAsync();
        //            }
        //            catch (Exception)
        //            {
        //                // 실패 시 롤백 (person만 생기는 것 방지)
        //                await transaction.RollbackAsync();
        //                throw; // 에러를 다시 던져서 500 응답으로 보냄
        //            }
        //        }
        //    }

        //    // 3. JWT 토큰 발급 (여기서 person_id를 심음)
        //    var tokens = await GenerateTokensAsync(person);

        //    // 4. 토큰 반환
        //    GuestLoginResponse res = new GuestLoginResponse()
        //    {
        //        accessToken = tokens.AccessToken,
        //        refreshToken = tokens.RefreshToken,
        //        displayPersonId = person.display_person_id
        //    };
        //    return Ok(new GameResponse<GuestLoginResponse>(res));
        //}

        //// POST: auth/refresh
        //// 토큰 갱신 (Access Token 만료 시 호출)
        //[HttpPost("refresh")]
        //public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest request)
        //{
        //    // personId 추출
        //    var principal = GetPrincipalFromExpiredToken(request.accessToken);
        //    if (principal == null)
        //    {
        //        return BadRequest("Invalid Access Token");
        //    }

        //    // claim에서 personId 확인
        //    var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        //    if (idClaim == null || !int.TryParse(idClaim.Value, out int personId))
        //    {
        //        return BadRequest("Invalid Token Claims");
        //    }

        //    // 저장된 Refresh Token
        //    string key = $"RT:{personId}";
        //    string savedRefreshToken = await _redisService.getAsync<string>(key);

        //    // 검증
        //    if (string.IsNullOrEmpty(savedRefreshToken) || savedRefreshToken != request.refreshToken)
        //    {
        //        return Unauthorized("Invalid Refresh Token"); // 재로그인 필요
        //    }

        //    // 유저 정보 교차 확인
        //    var person = await _personContext.person.FindAsync(personId);
        //    if (person == null) return Unauthorized("User not found");

        //    // 새 토큰 발급
        //    var newTokens = await GenerateTokensAsync(person);

        //    return Ok(new { accessToken = newTokens.AccessToken, refreshToken = newTokens.RefreshToken });
        //}

        //// --- Helper Methods ---
        //private async Task<string> GenerateUniqueDisplayIdAsync()
        //{
        //    while (true)
        //    {
        //        int randomNum = Random.Shared.Next(100000000, 999999999);
        //        string candidateId = randomNum.ToString();

        //        bool exists = await _personContext.person
        //                            .AnyAsync(p => p.display_person_id == int.Parse(candidateId));

        //        if (exists == false)
        //        {
        //            return candidateId;
        //        }
        //        // 4. 있으면(중복) 루프를 다시 돌아서 새 번호 뽑음

        //        // TODO: 동시성 이슈 발생 가능
        //    }
        //}

        //// 토큰 생성 및 Redis 저장 헬퍼
        //private async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(person person)
        //{
        //    // Access Token (JWT) 생성
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, person.person_id.ToString()),
        //        new Claim(ClaimTypes.Email, person.email ?? ""),
        //        new Claim("Hash", person.person_hash)
        //    };

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(30), // 수명: 30분
        //        signingCredentials: credentials);

        //    string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        //    // Refresh Token 생성 (랜덤 문자열)
        //    string refreshToken = Guid.NewGuid().ToString();

        //    // Redis에 Refresh Token 저장 (수명 14일)
        //    // Key: "RT:{personId}", Value: "{refreshToken}"
        //    string key = $"RT:{person.person_id}";
        //    await _redisService.setAsync<string>(key, refreshToken, TimeSpan.FromDays(14));

        //    return (accessToken, refreshToken);
        //}

        //// 만료된 JWT에서 정보(Claims)만 꺼내는 헬퍼
        //private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        //{
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = false, // 여기선 검증 안함
        //        ValidateIssuer = false,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
        //        ValidateLifetime = false // 만료되었어도 오류 안 나게 설정
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    try
        //    {
        //        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        //        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //            throw new SecurityTokenException("Invalid token");

        //        return principal;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}
