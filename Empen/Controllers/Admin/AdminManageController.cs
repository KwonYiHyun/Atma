using Empen.Data;
using Empen.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto.Admin;
using SharedData.Models;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers.Admin
{
    [Route("admin/manage")]
    [ApiController]
    [AdminApiKey]
    public class AdminManageController : ControllerBase
    {
        private readonly PersonDbContext _personContext;
        private readonly IRedisService _redisService;
        private readonly ITimeService _timeService;

        private const string TIME_KEY = "server:global_time_offset";

        public AdminManageController(PersonDbContext personContext, IRedisService redisService, ITimeService timeService)
        {
            _personContext = personContext;
            _redisService = redisService;
            _timeService = timeService;
        }

        // POST: admin/manage/cache/clear
        [HttpPost("cache/clear")]
        public async Task<ActionResult<GameResponse<string>>> ClearCache([FromBody] string targetType)
        {
            try
            {
                if (targetType == "ALL")
                {
                    await _redisService.deleteKeysByPatternAsync("Master:*");
                    await _redisService.publishAsync(CacheKey.Channel.Master, "ALL");
                }
                else
                {
                    string redisKey = $"Master:{targetType}:All";
                    await _redisService.deleteAsync(redisKey);
                    await _redisService.publishAsync(CacheKey.Channel.Master, targetType);
                }

                return Ok(new GameResponse<string>("캐시 초기화 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, ex.Message));
            }
        }

        // POST: admin/manage/time/get
        [HttpPost("time/get")]
        public async Task<ActionResult<GameResponse<long>>> GetTimeOffset()
        {
            string offsetStr = await _redisService.getAsync<string>(TIME_KEY);
            long ticks = long.TryParse(offsetStr, out long result) ? result : 0;
            return Ok(new GameResponse<long>(ticks));
        }

        // POST: admin/manage/time/set
        [HttpPost("time/set")]
        public async Task<ActionResult<GameResponse<string>>> SetTimeOffset([FromBody] long offsetTicks)
        {
            if (offsetTicks == 0)
            {
                await _redisService.deleteAsync(TIME_KEY);
            }
            else
            {
                await _redisService.setAsync<string>(TIME_KEY, offsetTicks.ToString(), TimeSpan.FromDays(1));
            }
            return Ok(new GameResponse<string>("서버 시간 변경 성공"));
        }

        // POST: admin/manage/maintenance/get
        [HttpPost("maintenance/get")]
        public async Task<ActionResult<GameResponse<bool>>> GetMaintenance()
        {
            string status = await _redisService.getAsync<string>(CacheTargets.MAINTENANCE_KEY);
            return Ok(new GameResponse<bool>(!string.IsNullOrEmpty(status)));
        }

        // POST: admin/manage/maintenance/set
        [HttpPost("maintenance/set")]
        public async Task<ActionResult<GameResponse<string>>> SetMaintenance([FromBody] bool isEnable)
        {
            if (isEnable)
            {
                await _redisService.setAsync<string>(CacheTargets.MAINTENANCE_KEY, "1", TimeSpan.FromDays(7));
            }
            else
            {
                await _redisService.deleteAsync(CacheTargets.MAINTENANCE_KEY);
            }
            return Ok(new GameResponse<string>("점검 상태 변경 성공"));
        }

        // POST: admin/manage/mail/send
        [HttpPost("mail/send")]
        public async Task<ActionResult<GameResponse<string>>> SendRewardMail([FromBody] PersonMailDto mailData)
        {
            try
            {
                if (mailData.person_id > 0)
                {
                    var newMail = new person_mail { 
                        person_mail_id = mailData.person_mail_id,
                        person_id = mailData.person_id,
                        title = mailData.title,
                        description = mailData.description,
                        reward_id_1 = mailData.reward_id_1,
                        reward_id_1_amount = mailData.reward_id_1_amount,
                        reward_id_2 = mailData.reward_id_2,
                        reward_id_2_amount = mailData.reward_id_2_amount,
                        reward_id_3 = mailData.reward_id_3,
                        reward_id_3_amount = mailData.reward_id_3_amount,
                        is_receive = mailData.is_receive,
                        expire_date = mailData.expire_date,
                        insert_date = mailData.insert_date,
                        update_date = mailData.update_date,
                    };
                    _personContext.person_mail.Add(newMail);
                    await _personContext.SaveChangesAsync();
                }
                else
                {
                    var now = await _timeService.getNowAsync();

                    FormattableString sql = $@"
                        INSERT INTO person_mail (
                            person_id, 
                            title, 
                            description, 
                            reward_id_1, 
                            reward_id_1_amount, 
                            reward_id_2, 
                            reward_id_2_amount, 
                            reward_id_3, 
                            reward_id_3_amount, 
                            is_receive, 
                            insert_date, 
                            update_date, 
                            expire_date
                        )
                        SELECT 
                            person_id, 
                            {mailData.title}, 
                            {mailData.description}, 
                            {mailData.reward_id_1}, 
                            {mailData.reward_id_1_amount}, 
                            {mailData.reward_id_2},
                            {mailData.reward_id_2_amount}, 
                            {mailData.reward_id_3}, 
                            {mailData.reward_id_3_amount}, 
                            0,
                            {now}, 
                            {now}, 
                            {mailData.expire_date}
                        FROM person_status;
                    ";

                    int affectedRows = await _personContext.Database.ExecuteSqlInterpolatedAsync(sql);
                }

                return Ok(new GameResponse<string>("발송 완료"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, ex.Message));
            }
        }
    }
}