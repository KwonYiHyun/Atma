using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Response;
using SharedData.Type;
using System.Security.Claims;

namespace Empen.Controllers
{
    [Authorize]
    [Route("character")]
    [ApiController]
    public class CharacterController : BaseController
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        // POST: character/list
        [HttpPost("list")]
        public async Task<ActionResult<GameResponse<ICollection<PersonCharacterDto>>>> getMyCharacterList()
        {
            var list = await _characterService.getMyCharacterListAsync(CurrentPersonId);

            return Ok(new GameResponse<ICollection<PersonCharacterDto>>(list));
        }

        // POST: character/{characterId}
        [HttpPost("{characterId}")]
        public async Task<ActionResult<GameResponse<CharacterDetailInfoDto>>> getCharacterDetailInfo(int characterId)
        {
            var (errorCode, info) = await _characterService.getCharacterDetailInfoAsync(CurrentPersonId, characterId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<CharacterDetailInfoDto>(errorCode, ""));
            }

            return Ok(new GameResponse<CharacterDetailInfoDto>(info));
        }

        // POST: character/levelupinfo/{characterId}
        [HttpPost("levelupinfo/{characterId}")]
        public async Task<ActionResult<GameResponse<CharacterLevelupInfoDto>>> getCharacterLevelupInfo(int characterId)
        {
            var (errorCode, info) = await _characterService.getCharacterLevelupInfoAsync(CurrentPersonId, characterId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<CharacterLevelupInfoDto>(errorCode, ""));
            }

            return Ok(new GameResponse<CharacterLevelupInfoDto>(info));
        }

        // POST: character/levelup/{characterId}
        [HttpPost("levelup/{characterId}")]
        public async Task<ActionResult<GameResponse<CharacterDetailInfoDto>>> characterLevelup(int characterId)
        {
            var (errorCode, levelup) = await _characterService.characterLevelUpAsync(CurrentPersonId, characterId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<CharacterDetailInfoDto>(errorCode, ""));
            }

            return Ok(new GameResponse<CharacterDetailInfoDto>(levelup));
        }

        // POST: character/limitbreak/{characterId}
        [HttpPost("limitbreak/{characterId}")]
        public async Task<ActionResult<GameResponse<CharacterDetailInfoDto>>> characterLimitBreak(int characterId)
        {
            var (errorCode, limitBreak) = await _characterService.characterLimitBreakAsync(CurrentPersonId, characterId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<CharacterDetailInfoDto>(errorCode, ""));
            }

            return Ok(new GameResponse<CharacterDetailInfoDto>(limitBreak));
        }
    }
}
