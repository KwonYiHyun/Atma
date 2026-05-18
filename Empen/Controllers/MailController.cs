using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Response;

namespace Empen.Controllers
{
    [Authorize]
    [Route("mail")]
    [ApiController]
    public class MailController : BaseController
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        // POST: mail/all
        [HttpPost("all")]
        public async Task<ActionResult<GameResponse<ICollection<MailInfoDto>>>> getAllMail()
        {
            var mails = await _mailService.getAllMail(CurrentPersonId);

            return Ok(new GameResponse<ICollection<MailInfoDto>>(mails));
        }

        // POST: mail/all/take
        [HttpPost("all/take")]
        public async Task<ActionResult<ICollection<ObjectDisplayDto>>> takeAllMailDisplay()
        {
            var mails = await _mailService.takeAllMailDisplay(CurrentPersonId);

            return Ok(new GameResponse<ICollection<ObjectDisplayDto>>(mails));
        }

        // POST: mail/take/{personMailId}
        [HttpPost("take/{personMailId}")]
        public async Task<ActionResult<ICollection<ObjectDisplayDto>>> takeMailById(int personMailId)
        {
            var mails = await _mailService.takeMailById(CurrentPersonId, personMailId);

            return Ok(new GameResponse<ICollection<ObjectDisplayDto>>(mails));
        }
    }
}
