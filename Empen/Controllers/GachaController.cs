using Empen;
using Empen.Filter;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Request;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers
{
    [Authorize]
    [Route("gacha")]
    [ApiController]
    public class GachaController : BaseController
    {
        private readonly IGachaService _gachaService;

        public GachaController(IGachaService gachaService)
        {
            _gachaService = gachaService;
        }

        // POST: gacha/all
        [HttpPost("all")]
        public async Task<ActionResult<GameResponse<ICollection<GachaInfoDto>>>> getAllGachaInfo()
        {
            var gachas = await _gachaService.getAllGachaAsync();

            return Ok(new GameResponse<ICollection<GachaInfoDto>>(gachas));
        }

        // POST: gacha/{gachaId}
        [HttpPost("{gachaId}")]
        public async Task<ActionResult<GameResponse<GachaInfoDto>>> getGachaInfoById(int gachaId)
        {
            var (errorCode, gacha) = await _gachaService.getGachaByIdAsync(gachaId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<GachaInfoDto>(errorCode, "가챠 정보가 없습니다."));
            }

            return Ok(new GameResponse<GachaInfoDto>(gacha));
        }

        // POST: gacha/play
        [HttpPost("play")]
        [AchievementCheck]
        public async Task<ActionResult<GameResponse<ICollection<GachaPlayInfoDto>>>> playGacha([FromBody] GachaPlayRequest query)
        {
            var (status, rewards) = await _gachaService.playGacha(CurrentPersonId, query.gachaId, query.execCount);
            if (status != ErrorCode.Success)
            {
                return Ok(new GameResponse<ICollection<GachaPlayInfoDto>>(status));
            }

            return Ok(new GameResponse<ICollection<GachaPlayInfoDto>>(rewards));
        }
    }
}
