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
    [Route("admin/loginbonusday")]
    [ApiController]
    [AdminApiKey]
    public class AdminLoginbonusDayController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminLoginbonusDayController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/loginbonusday/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterDailyLoginBonusDayDto>>>> getLoginbonuses()
        {
            var loginbonuses = await _masterContext.master_daily_login_bonus_day
                .AsNoTracking()
                .Select(l => new MasterDailyLoginBonusDayDto
                {
                    daily_login_bonus_day_id = l.daily_login_bonus_day_id,
                    daily_login_bonus_id = l.daily_login_bonus_id,
                    total_login_count = l.total_login_count,
                    reward_id_1 = l.reward_id_1,
                    start_date = l.start_date,
                    end_date = l.end_date,
                    insert_date = l.insert_date,
                    update_date = l.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterDailyLoginBonusDayDto>>(loginbonuses));
        }

        // POST: admin/loginbonusday/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editLoginbonusDay([FromBody] MasterDailyLoginBonusDayDto loginbonusDto)
        {
            var loginbonus = await _masterContext.master_daily_login_bonus_day.FirstOrDefaultAsync(p => p.daily_login_bonus_day_id == loginbonusDto.daily_login_bonus_day_id);
            if (loginbonus == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "패키지가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            loginbonus.daily_login_bonus_id = loginbonusDto.daily_login_bonus_id;
            loginbonus.total_login_count = loginbonusDto.total_login_count;
            loginbonus.reward_id_1 = loginbonusDto.reward_id_1;
            loginbonus.start_date = loginbonusDto.start_date;
            loginbonus.end_date = loginbonusDto.end_date;
            loginbonus.update_date = loginbonusDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.LoginbonusDayAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "LOGINBONUS_DAY");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/loginbonusday/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addLoginbonusDay([FromBody] MasterDailyLoginBonusDayDto loginbonusDto)
        {
            bool exists = await _masterContext.master_daily_login_bonus_day.AnyAsync(l => l.daily_login_bonus_day_id == loginbonusDto.daily_login_bonus_day_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newLoginbonus = new master_daily_login_bonus_day
            {
                daily_login_bonus_day_id = loginbonusDto.daily_login_bonus_day_id,
                daily_login_bonus_id = loginbonusDto.daily_login_bonus_id,
                total_login_count = loginbonusDto.total_login_count,
                reward_id_1 = loginbonusDto.reward_id_1,
                start_date = loginbonusDto.start_date,
                end_date = loginbonusDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_daily_login_bonus_day.Add(newLoginbonus);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.LoginbonusDayAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "LOGINBONUS_DAY");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
