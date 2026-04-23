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
    [Route("admin/loginbonus")]
    [ApiController]
    [AdminApiKey]
    public class AdminLoginbonusController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminLoginbonusController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/loginbonus/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterDailyLoginBonusDto>>>> getLoginbonuses()
        {
            var loginbonuses = await _masterContext.master_daily_login_bonus
                .AsNoTracking()
                .Select(l => new MasterDailyLoginBonusDto
                {
                    daily_login_bonus_id = l.daily_login_bonus_id,
                    title_text = l.title_text,
                    back_image = l.back_image,
                    start_date = l.start_date,
                    end_date = l.end_date,
                    insert_date = l.insert_date,
                    update_date = l.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterDailyLoginBonusDto>>(loginbonuses));
        }

        // POST: admin/loginbonus/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editLoginbonus([FromBody] MasterDailyLoginBonusDto loginbonusDto)
        {
            var loginbonus = await _masterContext.master_daily_login_bonus.FirstOrDefaultAsync(p => p.daily_login_bonus_id == loginbonusDto.daily_login_bonus_id);
            if (loginbonus == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "패키지가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            loginbonus.title_text = loginbonusDto.title_text;
            loginbonus.back_image = loginbonusDto.back_image;
            loginbonus.start_date = loginbonusDto.start_date;
            loginbonus.end_date = loginbonusDto.end_date;
            loginbonus.update_date = loginbonusDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.LoginbonusAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "LOGINBONUS");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/loginbonus/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addLoginbonus([FromBody] MasterDailyLoginBonusDto loginbonusDto)
        {
            bool exists = await _masterContext.master_daily_login_bonus.AnyAsync(l => l.daily_login_bonus_id == loginbonusDto.daily_login_bonus_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newLoginbonus = new master_daily_login_bonus
            {
                daily_login_bonus_id = loginbonusDto.daily_login_bonus_id,
                title_text = loginbonusDto.title_text,
                back_image = loginbonusDto.back_image,
                start_date = loginbonusDto.start_date,
                end_date = loginbonusDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_daily_login_bonus.Add(newLoginbonus);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.LoginbonusAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "LOGINBONUS");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
