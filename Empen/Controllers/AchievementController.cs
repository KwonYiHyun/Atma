using Empen.Service;
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
    [Route("achievement")]
    [ApiController]
    public class AchievementController : BaseController
    {
        private readonly IAchievementService _achievementService;

        public AchievementController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        // POST: achievement/all
        [HttpPost("all")]
        public async Task<ActionResult<GameResponse<ICollection<AchievementDto>>>> getAllAchievement()
        {
            var list = await _achievementService.getAllAchievementAsync(CurrentPersonId);

            return Ok(new GameResponse<ICollection<AchievementDto>>(list));
        }

        // POST: achievement/claim/{achievementId}
        [HttpPost("claim/{achievementId}")]
        public async Task<ActionResult<GameResponse<ICollection<ObjectDisplayDto>>>> claimAchievement(int achievementId)
        {
            var (errorCode, result) = await _achievementService.claimAchievementAsync(CurrentPersonId, achievementId);
            
            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<ICollection<ObjectDisplayDto>>(errorCode, ""));
            }

            return Ok(new GameResponse<ICollection<ObjectDisplayDto>>(result));
        }

        // POST: achievement/claim/all
        [HttpPost("claim/all")]
        public async Task<ActionResult<GameResponse<ICollection<ObjectDisplayDto>>>> claimAllAchievement()
        {
            var (errorCode, result) = await _achievementService.claimAllAchievementAsync(CurrentPersonId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<ICollection<ObjectDisplayDto>>(errorCode, ""));
            }

            return Ok(new GameResponse<ICollection<ObjectDisplayDto>>(result));
        }
    }
}
