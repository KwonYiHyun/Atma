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
    [Route("admin/characterlevel")]
    [ApiController]
    [AdminApiKey]
    public class AdminCharacterLevelController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminCharacterLevelController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/characterlevel/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterCharacterLevelDto>>>> getCharacterLevels()
        {
            var characterLevels = await _masterContext.master_character_level
                .AsNoTracking()
                .Select(c => new MasterCharacterLevelDto
                {
                    character_level_id = c.character_level_id,
                    character_id = c.character_id,
                    level = c.level,
                    atk = c.atk,
                    def = c.def,
                    hp = c.hp,
                    stance = c.stance,
                    resource_item_id_1 = c.resource_item_id_1,
                    resource_item_id_2 = c.resource_item_id_2,
                    resource_item_id_3 = c.resource_item_id_3,
                    item_1_amount = c.item_1_amount,
                    item_2_amount = c.item_2_amount,
                    item_3_amount = c.item_3_amount,
                    start_date = c.start_date,
                    end_date = c.end_date,
                    insert_date = c.insert_date,
                    update_date = c.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterCharacterLevelDto>>(characterLevels));
        }

        // POST: admin/characterlevel/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editCharacterLevel([FromBody] MasterCharacterLevelDto characterLevelDto)
        {
            var characterLevel = await _masterContext.master_character_level.FirstOrDefaultAsync(c => c.character_id == characterLevelDto.character_id);
            if (characterLevel == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "레벨 정보가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            characterLevel.character_id = characterLevelDto.character_id;
            characterLevel.level = characterLevelDto.level;
            characterLevel.atk = characterLevelDto.atk;
            characterLevel.def = characterLevelDto.def;
            characterLevel.hp = characterLevelDto.hp;
            characterLevel.stance = characterLevelDto.stance;
            characterLevel.resource_item_id_1 = characterLevelDto.resource_item_id_1;
            characterLevel.resource_item_id_2 = characterLevelDto.resource_item_id_2;
            characterLevel.resource_item_id_3 = characterLevelDto.resource_item_id_3;
            characterLevel.item_1_amount = characterLevelDto.item_1_amount;
            characterLevel.item_2_amount = characterLevelDto.item_2_amount;
            characterLevel.item_3_amount = characterLevelDto.item_3_amount;
            characterLevel.start_date = characterLevelDto.start_date;
            characterLevel.end_date = characterLevelDto.end_date;
            characterLevel.update_date = characterLevelDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.CharacterLevelAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "CHARACTER_LEVEL");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/characterlevel/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addCharacterLevel([FromBody] MasterCharacterLevelDto characterLevelDto)
        {
            bool exists = await _masterContext.master_character_level.AnyAsync(c => c.character_level_id == characterLevelDto.character_level_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newCharacterLevel = new master_character_level
            {
                character_id = characterLevelDto.character_id,
                level = characterLevelDto.level,
                atk = characterLevelDto.atk,
                def = characterLevelDto.def,
                hp = characterLevelDto.hp,
                stance = characterLevelDto.stance,
                resource_item_id_1 = characterLevelDto.resource_item_id_1,
                resource_item_id_2 = characterLevelDto.resource_item_id_2,
                resource_item_id_3 = characterLevelDto.resource_item_id_3,
                item_1_amount = characterLevelDto.item_1_amount,
                item_2_amount = characterLevelDto.item_2_amount,
                item_3_amount = characterLevelDto.item_3_amount,
                start_date = characterLevelDto.start_date,
                end_date = characterLevelDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_character_level.Add(newCharacterLevel);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.CharacterLevelAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "CHARACTER_LEVEL");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}