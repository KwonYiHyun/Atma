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
    [Route("admin/notice")]
    [ApiController]
    [AdminApiKey]
    public class AdminNoticeController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminNoticeController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/notice/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterNoticeDto>>>> getNotices()
        {
            var notices = await _masterContext.master_notice
                .AsNoTracking()
                .Select(n => new MasterNoticeDto
                {
                    notice_id = n.notice_id,
                    notice_title = n.notice_title,
                    notice_content = n.notice_content,
                    notice_banner_image = n.notice_banner_image,
                    notice_link = n.notice_link,
                    notice_type = n.notice_type,
                    show_order = n.show_order,
                    start_date = n.start_date,
                    end_date = n.end_date,
                    insert_date = n.insert_date,
                    update_date = n.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterNoticeDto>>(notices));
        }

        // POST: admin/notice/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editCharacterGrade([FromBody] MasterNoticeDto noticeDto)
        {
            var notice = await _masterContext.master_notice.FirstOrDefaultAsync(n => n.notice_id == noticeDto.notice_id);
            if (notice == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "공지가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            notice.notice_title = noticeDto.notice_title;
            notice.notice_content = noticeDto.notice_content;
            notice.notice_banner_image = noticeDto.notice_banner_image;
            notice.notice_link = noticeDto.notice_link;
            notice.notice_type = noticeDto.notice_type;
            notice.show_order = noticeDto.show_order;
            notice.start_date = noticeDto.start_date;
            notice.end_date = noticeDto.end_date;
            notice.update_date = noticeDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.NoticeAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "NOTICE");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/notice/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addCharacterGrade([FromBody] MasterNoticeDto noticeDto)
        {
            bool exists = await _masterContext.master_notice.AnyAsync(n => n.notice_id == noticeDto.notice_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newNotice = new master_notice
            {
                notice_id = noticeDto.notice_id,
                notice_title = noticeDto.notice_title,
                notice_content = noticeDto.notice_content,
                notice_banner_image = noticeDto.notice_banner_image,
                notice_link = noticeDto.notice_link,
                notice_type = noticeDto.notice_type,
                show_order = noticeDto.show_order,
                start_date = noticeDto.start_date,
                end_date = noticeDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_notice.Add(newNotice);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.NoticeAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "NOTICE");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
