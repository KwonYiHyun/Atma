using Empen.Service;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Response;

namespace Empen.Controllers
{
    [Authorize]
    [Route("notice")]
    [ApiController]
    public class NoticeController : BaseController
    {
        private readonly INoticeService _noticeService;

        public NoticeController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        // POST: notice/all
        [HttpPost("all")]
        public async Task<ActionResult<GameResponse<ICollection<NoticeInfoDto>>>> getAllNotice()
        {
            var notices = await _noticeService.getAllNotice();

            return Ok(new GameResponse<ICollection<NoticeInfoDto>>(notices));
        }
    }
}
