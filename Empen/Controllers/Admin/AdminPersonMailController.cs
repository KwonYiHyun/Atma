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

        public AdminPersonMailController(PersonDbContext personContext, ITimeService timeService)
        {
            _personContext = personContext;
            _timeService = timeService;
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
            DateTime now = await _timeService.getNowAsync();

            var updatedRows = await _personContext.person_mail
                .Where(m => m.person_mail_id == dto.person_mail_id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.title, dto.title)
                    .SetProperty(p => p.description, dto.description)
                    .SetProperty(p => p.is_receive, dto.is_receive)
                    .SetProperty(p => p.expire_date, dto.expire_date)
                    .SetProperty(p => p.update_date, now)
                );

            if (updatedRows == 0)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정할 우편이 없습니다."));
            }
            return Ok(new GameResponse<string>("수정 성공"));
        }

        // POST: admin/person_mail/delete/{personMailId}
        [HttpPost("delete/{personMailId}")]
        public async Task<ActionResult<GameResponse<string>>> DeletePersonMail(int personMailId)
        {
            var deletedRows = await _personContext.person_mail
                .Where(m => m.person_mail_id == personMailId)
                .ExecuteDeleteAsync();

            if (deletedRows == 0)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "삭제할 우편이 없습니다."));
            }
            return Ok(new GameResponse<string>("삭제 성공"));
        }
    }
}