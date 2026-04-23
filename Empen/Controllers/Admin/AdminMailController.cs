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
    [Route("admin/mail")]
    [ApiController]
    [AdminApiKey]
    public class AdminMailController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminMailController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/mail/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterMailDto>>>> getMails()
        {
            var mails = await _masterContext.master_mail
                .AsNoTracking()
                .Select(m => new MasterMailDto
                {
                    mail_id = m.mail_id,
                    sender_type = m.sender_type,
                    mail_subject = m.mail_subject,
                    mail_body = m.mail_body,
                    important_flag = m.important_flag,
                    start_date = m.start_date,
                    end_date = m.end_date,
                    insert_date = m.insert_date,
                    update_date = m.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterMailDto>>(mails));
        }

        // POST: admin/mail/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editMail([FromBody] MasterMailDto mailDto)
        {
            var mail = await _masterContext.master_mail.FirstOrDefaultAsync(m => m.mail_id == mailDto.mail_id);
            if (mail == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "업적이 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            mail.sender_type = mailDto.sender_type;
            mail.mail_subject = mailDto.mail_subject;
            mail.mail_body = mailDto.mail_body;
            mail.important_flag = mailDto.important_flag;
            mail.start_date = mailDto.start_date;
            mail.end_date = mailDto.end_date;
            mail.update_date = mailDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.MailAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "MAIL");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/mail/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addMail([FromBody] MasterMailDto mailDto)
        {
            bool exists = await _masterContext.master_mail.AnyAsync(m => m.mail_id == mailDto.mail_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newMail = new master_mail
            {
                mail_id = mailDto.mail_id,
                sender_type = mailDto.sender_type,
                mail_subject = mailDto.mail_subject,
                mail_body = mailDto.mail_body,
                important_flag = mailDto.important_flag,
                start_date = mailDto.start_date,
                end_date = mailDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_mail.Add(newMail);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.MailAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "MAIL");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
