using Empen.Service;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Request;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers
{
    [Authorize]
    [Route("personinfo")]
    [ApiController]
    public class PersonInfoController : BaseController
    {
        private readonly IPersonService _personInfoService;

        public PersonInfoController(IPersonService personInfoService)
        {
            _personInfoService = personInfoService;
        }

        // POST: personinfo/get
        [HttpPost("get")]
        public async Task<ActionResult<PersonInfoDto>> getPersonInfo()
        {
            var info = await _personInfoService.getPersonInfo(CurrentPersonId);

            if (info == null)
            {
                return Ok(new GameResponse<PersonInfoDto>(ErrorCode.PersonNotFound, "플레이어가 없습니다."));
            }

            return Ok(new GameResponse<PersonInfoDto>(info));
        }

        // POST: personinfo/set
        [HttpPost("set")]
        public async Task<ActionResult<GameResponse<string>>> setPersonInfo([FromBody] PersonInfoRequest query)
        {
            var result = await _personInfoService.setPersonInfo(CurrentPersonId, query);

            if (result != ErrorCode.Success)
            {
                return new GameResponse<string>(result, "설정 실패");
            }

            return new GameResponse<string>("설정 성공");
        }

        // POST: personinfo/set/leader/{characterId}
        [HttpPost("set/leader/{characterId}")]
        public async Task<ActionResult<GameResponse<string>>> setPersonLeaderCharacter(int characterId)
        {
            var result = await _personInfoService.setPersonLeaderCharacter(CurrentPersonId, characterId);

            if (result != ErrorCode.Success)
            {
                return new GameResponse<string>(result, "리더 설정 실패");
            }

            return Ok(new GameResponse<string>("리더 설정 성공"));
        }
    }
}
