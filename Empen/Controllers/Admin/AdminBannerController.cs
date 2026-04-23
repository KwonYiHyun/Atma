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
using System.Runtime.InteropServices.JavaScript;

namespace Empen.Controllers.Admin
{
    [Route("admin/banner")]
    [ApiController]
    [AdminApiKey]
    public class AdminBannerController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminBannerController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/banner/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterBannerDto>>>> getBanners()
        {
            var banners = await _masterContext.master_banner
                .AsNoTracking()
                .Select(b => new MasterBannerDto
                {
                    banner_id = b.banner_id,
                    show_order = b.show_order,
                    show_place_type = b.show_place_type,
                    banner_image = b.banner_image,
                    banner_action_type = b.banner_action_type,
                    banner_action_param = b.banner_action_param,
                    limited_type = b.limited_type,
                    limited_param = b.limited_param,
                    os_type = b.os_type,
                    start_date = b.start_date,
                    end_date = b.end_date,
                    insert_date = b.insert_date,
                    update_date = b.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterBannerDto>>(banners));
        }

        // POST: admin/banner/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editBanner([FromBody] MasterBannerDto bannerDto)
        {
            var banner = await _masterContext.master_banner.FirstOrDefaultAsync(b => b.banner_id == bannerDto.banner_id);
            if (banner == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "배너가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            banner.show_order = bannerDto.show_order;
            banner.show_place_type = bannerDto.show_place_type;
            banner.banner_image = bannerDto.banner_image;
            banner.banner_action_type = bannerDto.banner_action_type;
            banner.banner_action_param = bannerDto.banner_action_param;
            banner.limited_type = bannerDto.limited_type;
            banner.limited_param = bannerDto.limited_param;
            banner.os_type = bannerDto.os_type;
            banner.start_date = bannerDto.start_date;
            banner.end_date = bannerDto.end_date;
            banner.update_date = now;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.BannerAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "BANNER");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/banner/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addBanner([FromBody] MasterBannerDto bannerDto)
        {
            bool exists = await _masterContext.master_banner.AnyAsync(b => b.banner_id == bannerDto.banner_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newBanner = new master_banner
            {
                banner_id = bannerDto.banner_id,
                show_order = bannerDto.show_order,
                show_place_type = bannerDto.show_place_type,
                banner_image = bannerDto.banner_image,
                banner_action_type = bannerDto.banner_action_type,
                banner_action_param = bannerDto.banner_action_param,
                limited_type = bannerDto.limited_type,
                limited_param = bannerDto.limited_param,
                os_type = bannerDto.os_type,
                start_date = bannerDto.start_date,
                end_date = bannerDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_banner.Add(newBanner);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.BannerAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "BANNER");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }

        }
    }
}
