using Empen.Data;
using Empen.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto.Admin;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers.Admin
{
    [Route("admin/person_mail")]
    [ApiController]
    [AdminApiKey]
    public class AdminPersonMailController : ControllerBase
    {
        private readonly PersonDbContext _personContext;
        private readonly ITimeService _timeService;
        private readonly IRedisLockService _redisLockService;
        private readonly ILogger<AdminPersonMailController> _logger;

        public AdminPersonMailController(PersonDbContext personContext, ITimeService timeService, IRedisLockService redisLockService, ILogger<AdminPersonMailController> logger)
        {
            _personContext = personContext;
            _timeService = timeService;
            _redisLockService = redisLockService;
            _logger = logger;
        }

        // POST: admin/person_mail/get/{personId}
        [HttpPost("get/{personId}")]
        public async Task<ActionResult<GameResponse<List<PersonMailDto>>>> GetPersonMails(int personId)
        {
            var mails = await _personContext.person_mail
                .AsNoTracking()
                .Where(m => m.person_id == personId)
                .Select(m => new PersonMailDto
                {
                    person_mail_id = m.person_mail_id,
                    person_id = m.person_id,
                    title = m.title,
                    description = m.description,
                    reward_id_1 = m.reward_id_1,
                    reward_id_1_amount = m.reward_id_1_amount,
                    reward_id_2 = m.reward_id_2,
                    reward_id_2_amount = m.reward_id_2_amount,
                    reward_id_3 = m.reward_id_3,
                    reward_id_3_amount = m.reward_id_3_amount,
                    is_receive = m.is_receive,
                    expire_date = m.expire_date,
                    insert_date = m.insert_date,
                    update_date = m.update_date
                })
                .ToListAsync();

            return Ok(new GameResponse<List<PersonMailDto>>(mails));
        }

        // POST: admin/person_mail/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> EditPersonMail([FromBody] PersonMailDto dto)
        {
            var mailOwner = await _personContext.person_mail
                .AsNoTracking()
                .Where(m => m.person_mail_id == dto.person_mail_id)
                .Select(m => new { m.person_id })
                .FirstOrDefaultAsync();

            if (mailOwner == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정할 우편이 없습니다."));
            }

            // 분산락
            string lockKey = mailOwner.person_id.ToString();
            string? lockToken = await _redisLockService.lockAsync(lockKey, 3, TimeSpan.FromSeconds(1));

            if (lockToken == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.UserDataLocked, "유저가 현재 사용 중입니다. 잠시 후 다시 시도해주세요."));
            }

            try
            {
                // 분산락 잡은 후 데이터가 최신인지 확인
                var latestMail = await _personContext.person_mail
                    .Where(m => m.person_mail_id == dto.person_mail_id)
                    .FirstOrDefaultAsync();

                if (latestMail == null)
                {
                    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "조회 중 우편이 삭제되었습니다."));
                }

                if (latestMail.is_receive != dto.is_receive)
                {
                    return Ok(new GameResponse<string>(ErrorCode.AlreadyProcessed, "데이터 상태가 변경되었습니다. 다시 조회 후 시도해주세요."));
                }

                if (latestMail.update_date != dto.update_date)
                {
                    return Ok(new GameResponse<string>(ErrorCode.AlreadyProcessed, "데이터 상태가 변경되었습니다. 다시 조회 후 시도해주세요."));
                }

                DateTime now = await _timeService.getNowAsync();

                latestMail.title = dto.title;
                latestMail.description = dto.description;
                latestMail.is_receive = dto.is_receive;
                latestMail.expire_date = dto.expire_date;
                latestMail.update_date = now;

                await _personContext.SaveChangesAsync();

                return Ok(new GameResponse<string>("수정 성공"));
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"EditPersonMail Exception! MailId: {dto.person_mail_id}");
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "수정 중 에러가 발생했습니다."));
            }
            finally
            {
                await _redisLockService.unLockAsync(lockKey, lockToken);
            }

            // 기존 방식
            //DateTime now = await _timeService.getNowAsync();

            //var updatedRows = await _personContext.person_mail
            //    .Where(m => m.person_mail_id == dto.person_mail_id)
            //    .ExecuteUpdateAsync(s => s
            //        .SetProperty(p => p.title, dto.title)
            //        .SetProperty(p => p.description, dto.description)
            //        .SetProperty(p => p.is_receive, dto.is_receive)
            //        .SetProperty(p => p.expire_date, dto.expire_date)
            //        .SetProperty(p => p.update_date, now)
            //    );

            //if (updatedRows == 0)
            //{
            //    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정할 우편이 없습니다."));
            //}
            //return Ok(new GameResponse<string>("수정 성공"));
        }

        // POST: admin/person_mail/delete
        [HttpPost("delete")]
        public async Task<ActionResult<GameResponse<string>>> DeletePersonMail([FromBody] PersonMailDeleteDto dto)
        {
            var mailOwner = await _personContext.person_mail
                .AsNoTracking()
                .Where(m => m.person_mail_id == dto.person_mail_id)
                .Select(m => new { m.person_id })
                .FirstOrDefaultAsync();

            if (mailOwner == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "삭제할 우편이 없습니다."));
            }

            // 분산락
            string lockKey = mailOwner.person_id.ToString();
            string? lockToken = await _redisLockService.lockAsync(lockKey, 3, TimeSpan.FromSeconds(1));

            if (lockToken == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.UserDataLocked, "유저가 사용 중이라 삭제할 수 없습니다."));
            }

            try
            {
                // 분산락 잡은 후 데이터가 최신인지 확인
                var latestMail = await _personContext.person_mail
                    .Where(m => m.person_mail_id == dto.person_mail_id)
                    .FirstOrDefaultAsync();

                if (latestMail == null)
                {
                    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "이미 데이터가 삭제되었습니다."));
                }

                if (latestMail.is_receive)
                {
                    return Ok(new GameResponse<string>(ErrorCode.AlreadyProcessed, "이미 수령 완료된 우편은 삭제할 수 없습니다."));
                }

                if (dto.update_date != latestMail.update_date)
                {
                    return Ok(new GameResponse<string>(ErrorCode.AlreadyProcessed, "데이터가 변경되었습니다. 다시 조회 후 시도해주세요."));
                }

                _personContext.person_mail.Remove(latestMail);
                await _personContext.SaveChangesAsync();
                _logger.LogInformation($"DeletePersonMail MailId: {latestMail.person_mail_id} PersonId: {latestMail.person_id} Title: {latestMail.title}");
                return Ok(new GameResponse<string>("삭제 성공"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeletePersonMail Exception! MailId: {dto.person_mail_id}");
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "삭제 중 에러가 발생했습니다."));
            }
            finally
            {
                await _redisLockService.unLockAsync(lockKey, lockToken);
            }

            // 기존 방식
            //var deletedRows = await _personContext.person_mail
            //    .Where(m => m.person_mail_id == personMailId)
            //    .ExecuteDeleteAsync();

            //if (deletedRows == 0)
            //{
            //    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "삭제할 우편이 없습니다."));
            //}
            //return Ok(new GameResponse<string>("삭제 성공"));
        }
    }
}