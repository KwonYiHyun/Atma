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
    [Route("admin/charactergrade")]
    [ApiController]
    [AdminApiKey]
    public class AdminCharacterGradeController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminCharacterGradeController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/charactergrade/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterCharacterGradeDto>>>> getCharacterGrades()
        {
            var characterGrades = await _masterContext.master_character_grade
                .AsNoTracking()
                .Select(c => new MasterCharacterGradeDto
                {
                    character_grade_id = c.character_grade_id,
                    character_id = c.character_id,
                    grade = c.grade,
                    critical_rate = c.critical_rate,
                    critical_damage = c.critical_damage,
                    require_count = c.require_count,
                    price_token = c.price_token,
                    start_date = c.start_date,
                    end_date = c.end_date,
                    insert_date = c.insert_date,
                    update_date = c.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterCharacterGradeDto>>(characterGrades));
        }

        // POST: admin/charactergrade/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editCharacterGrade([FromBody] MasterCharacterGradeDto characterGradeDto)
        {
            var characterGrade = await _masterContext.master_character_grade.FirstOrDefaultAsync(c => c.character_grade_id == characterGradeDto.character_grade_id);
            if (characterGrade == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "등급 정보가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            characterGrade.character_id = characterGradeDto.character_id;
            characterGrade.grade = characterGradeDto.grade;
            characterGrade.critical_rate = characterGradeDto.critical_rate;
            characterGrade.critical_damage = characterGradeDto.critical_damage;
            characterGrade.require_count = characterGradeDto.require_count;
            characterGrade.price_token = characterGradeDto.price_token;
            characterGrade.start_date = characterGradeDto.start_date;
            characterGrade.end_date = characterGradeDto.end_date;
            characterGrade.update_date = characterGradeDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.CharacterGradeAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "CHARACTER_GRADE");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/charactergrade/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addCharacterGrade([FromBody] MasterCharacterGradeDto characterGradeDto)
        {
            bool exists = await _masterContext.master_character_grade.AnyAsync(c => c.character_grade_id == characterGradeDto.character_grade_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newCharacterLevel = new master_character_grade
            {
                character_grade_id = characterGradeDto.character_grade_id,
                character_id = characterGradeDto.character_id,
                grade = characterGradeDto.grade,
                critical_rate = characterGradeDto.critical_rate,
                critical_damage = characterGradeDto.critical_damage,
                require_count = characterGradeDto.require_count,
                price_token = characterGradeDto.price_token,
                start_date = characterGradeDto.start_date,
                end_date = characterGradeDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_character_grade.Add(newCharacterLevel);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.CharacterGradeAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "CHARACTER_GRADE");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
