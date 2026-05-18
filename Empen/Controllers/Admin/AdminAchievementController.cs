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
    [Route("admin/achievement")]
    [ApiController]
    [AdminApiKey]
    public class AdminAchievementController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminAchievementController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/achievement/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterAchievementDto>>>> getLoginbonuses()
        {
            var achievements = await _masterContext.master_achievement
                .AsNoTracking()
                .Select(a => new MasterAchievementDto
                {
                    achievement_id = a.achievement_id,
                    show_order = a.show_order,
                    achievement_category_id = a.achievement_category_id,
                    achievement_title = a.achievement_title,
                    achievement_type = a.achievement_type,
                    achievement_param_1 = a.achievement_param_1,
                    achievement_param_2 = a.achievement_param_2,
                    achievement_param_3 = a.achievement_param_3,
                    achievement_description = a.achievement_description,
                    reward_id_1 = a.reward_id_1,
                    reward_id_2 = a.reward_id_2,
                    reward_id_3 = a.reward_id_3,
                    start_date = a.start_date,
                    end_date = a.end_date,
                    insert_date = a.insert_date,
                    update_date = a.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterAchievementDto>>(achievements));
        }

        // POST: admin/achievement/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editLoginbonus([FromBody] MasterAchievementDto achievementDto)
        {
            var achievement = await _masterContext.master_achievement.FirstOrDefaultAsync(a => a.achievement_id == achievementDto.achievement_id);
            if (achievement == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "업적이 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            achievement.show_order = achievementDto.show_order;
            achievement.achievement_category_id = achievementDto.achievement_category_id;
            achievement.achievement_title = achievementDto.achievement_title;
            achievement.achievement_type = achievementDto.achievement_type;
            achievement.achievement_param_1 = achievementDto.achievement_param_1;
            achievement.achievement_param_2 = achievementDto.achievement_param_2;
            achievement.achievement_param_3 = achievementDto.achievement_param_3;
            achievement.achievement_description = achievementDto.achievement_description;
            achievement.reward_id_1 = achievementDto.reward_id_1;
            achievement.reward_id_2 = achievementDto.reward_id_2;
            achievement.reward_id_3 = achievementDto.reward_id_3;
            achievement.start_date = achievementDto.start_date;
            achievement.end_date = achievementDto.end_date;
            achievement.update_date = achievementDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.AchievementAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "ACHIEVEMENT");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, $"서버 에러 {ex}"));
            }
        }

        // POST: admin/achievement/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addLoginbonus([FromBody] MasterAchievementDto achievementDto)
        {
            bool exists = await _masterContext.master_achievement.AnyAsync(a => a.achievement_id == achievementDto.achievement_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newAchievement = new master_achievement
            {
                achievement_id = achievementDto.achievement_id,
                show_order = achievementDto.show_order,
                achievement_category_id = achievementDto.achievement_category_id,
                achievement_title = achievementDto.achievement_title,
                achievement_type = achievementDto.achievement_type,
                achievement_param_1 = achievementDto.achievement_param_1,
                achievement_param_2 = achievementDto.achievement_param_2,
                achievement_param_3 = achievementDto.achievement_param_3,
                achievement_description = achievementDto.achievement_description,
                reward_id_1 = achievementDto.reward_id_1,
                reward_id_2 = achievementDto.reward_id_2,
                reward_id_3 = achievementDto.reward_id_3,
                start_date = achievementDto.start_date,
                end_date = achievementDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_achievement.Add(newAchievement);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.AchievementAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "ACHIEVEMENT");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
