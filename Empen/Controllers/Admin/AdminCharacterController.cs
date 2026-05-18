using Empen.Data;
using Empen.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto.Admin;
using SharedData.Models;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers.Admin
{
    [Route("admin/character")]
    [ApiController]
    [AdminApiKey]
    public class AdminCharacterController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminCharacterController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/character/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterCharacterDto>>>> getCharacters()
        {
            var characters = await _masterContext.master_character
                .AsNoTracking()
                .Select(c => new MasterCharacterDto
                {
                    character_id = c.character_id,
                    show_order = c.show_order,
                    character_name = c.character_name,
                    character_level_max = c.character_level_max,
                    character_grade_max = c.character_grade_max,
                    character_grade = c.character_grade,
                    character_critical_rate = c.character_critical_rate,
                    character_critical_damage = c.character_critical_damage,
                    piece_item_id = c.piece_item_id,
                    piece_amount_duplicate = c.piece_amount_duplicate,
                    character_description = c.character_description,
                    character_comment_1 = c.character_comment_1,
                    character_comment_2 = c.character_comment_2,
                    character_comment_3 = c.character_comment_3,
                    character_comment_1_motion = c.character_comment_1_motion,
                    character_comment_2_motion = c.character_comment_2_motion,
                    character_comment_3_motion = c.character_comment_3_motion,
                    start_date = c.start_date,
                    end_date = c.end_date,
                    insert_date = c.insert_date,
                    update_date = c.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterCharacterDto>>(characters));
        }

        // POST: admin/character/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editCharacter([FromBody] MasterCharacterDto characterDto)
        {
            var character = await _masterContext.master_character.FirstOrDefaultAsync(c => c.character_id == characterDto.character_id);
            if (character == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "캐릭터가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            character.show_order = characterDto.show_order;
            character.character_name = characterDto.character_name;
            character.character_level_max = characterDto.character_level_max;
            character.character_grade_max = characterDto.character_grade_max;
            character.character_grade = characterDto.character_grade;
            character.character_critical_rate = characterDto.character_critical_rate;
            character.character_critical_damage = characterDto.character_critical_damage;
            character.piece_item_id = characterDto.piece_item_id;
            character.piece_amount_duplicate = characterDto.piece_amount_duplicate;
            character.character_description = characterDto.character_description;
            character.character_comment_1 = characterDto.character_comment_1;
            character.character_comment_2 = characterDto.character_comment_2;
            character.character_comment_3 = characterDto.character_comment_3;
            character.character_comment_1_motion = characterDto.character_comment_1_motion;
            character.character_comment_2_motion = characterDto.character_comment_2_motion;
            character.character_comment_3_motion = characterDto.character_comment_3_motion;
            character.start_date = characterDto.start_date;
            character.end_date = characterDto.end_date;
            character.update_date = characterDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.CharacterAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "CHARACTER");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/character/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addCharacter([FromBody] MasterCharacterDto characterDto)
        {
            bool exists = await _masterContext.master_character.AnyAsync(c => c.character_id == characterDto.character_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newCharacter = new master_character
            {
                character_id = characterDto.character_id,
                show_order = characterDto.show_order,
                character_name = characterDto.character_name,
                character_level_max = characterDto.character_level_max,
                character_grade_max = characterDto.character_grade_max,
                character_grade = characterDto.character_grade,
                character_critical_rate = characterDto.character_critical_rate,
                character_critical_damage = characterDto.character_critical_damage,
                piece_item_id = characterDto.piece_item_id,
                piece_amount_duplicate = characterDto.piece_amount_duplicate,
                character_description = characterDto.character_description,
                character_comment_1 = characterDto.character_comment_1,
                character_comment_2 = characterDto.character_comment_2,
                character_comment_3 = characterDto.character_comment_3,
                character_comment_1_motion = characterDto.character_comment_1_motion,
                character_comment_2_motion = characterDto.character_comment_2_motion,
                character_comment_3_motion = characterDto.character_comment_3_motion,
                start_date = characterDto.start_date,
                end_date = characterDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_character.Add(newCharacter);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.CharacterAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "CHARACTER");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
