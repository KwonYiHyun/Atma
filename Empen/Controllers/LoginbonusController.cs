using Empen.Filter;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers
{
    [Authorize]
    [Route("loginbonus")]
    [ApiController]
    public class LoginbonusController : BaseController
    {
        private readonly ILoginbonusService _loginService;
        private readonly IAchievementService _achievementService;

        public LoginbonusController(ILoginbonusService loginService, IAchievementService achievementService)
        {
            _loginService = loginService;
            _achievementService = achievementService;
        }

        // POST: loginbonus/all
        [HttpPost("all")]
        public async Task<ActionResult<GameResponse<ICollection<LoginbonusInfoDto>>>> getAllLoginbonusList()
        {
            var logins = await _loginService.getAllLoginbonusAsync();

            return Ok(new GameResponse<ICollection<LoginbonusInfoDto>>(logins));
        }

        // POST: loginbonus/active
        [HttpPost("active")]
        public async Task<ActionResult<GameResponse<ICollection<LoginbonusInfoDto>>>> getActiveLoginbonusList()
        {
            var logins = await _loginService.getActiveLoginbonusAsync();
            
            return Ok(new GameResponse<ICollection<LoginbonusInfoDto>>(logins));
        }

        // POST: loginbonus/check
        [HttpPost("check")]
        [AchievementCheck]
        public async Task<ActionResult<ICollection<LoginbonusInfoDto>>> getAllLoginbonusByPersonId()
        {
            var logins = await _loginService.getAllLoginbonusByPersonIdAsync(CurrentPersonId);
            
            await _achievementService.checkAndSetNewFlagAsync(CurrentPersonId, AchievementType.LOGIN_COUNT);

            return Ok(new GameResponse<ICollection<LoginbonusInfoDto>>(logins));
        }
    }
}
