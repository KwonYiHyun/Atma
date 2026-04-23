using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;

namespace Empen.Service
{
    public class BannerService : IBannerService
    {
        private readonly IRedisService _redisService;
        private readonly ITimeService _timeService;
        private readonly IMasterDataCacheService _masterCacheService;

        public BannerService(IRedisService redisService, ITimeService timeService, IMasterDataCacheService masterCacheService)
        {
            _redisService = redisService;
            _timeService = timeService;
            _masterCacheService = masterCacheService;
        }

        public async Task<ICollection<BannerInfoDto>> getAllBanner()
        {
            DateTime now = await _timeService.getNowAsync();

            var bannerMap = await _masterCacheService.getBannerMapAsync();

            var bannerList = bannerMap.Values.ToList();

            var banners = bannerList.Select(b => new BannerInfoDto()
            {
                banner_id = b.banner_id,
                show_place_type = b.show_place_type,
                banner_image = b.banner_image,
                banner_action_type = (BannerActionType)b.banner_action_type,
                banner_action_param = b.banner_action_param,
                limited_type = b.limited_type,
                limited_param = b.limited_param,
                start_date = b.start_date,
                end_date = b.end_date
            })
            .Where(b => (b.start_date <= now && b.end_date >= now) || (b.start_date <= now && b.end_date == null))
            .ToList();

            return banners;
        }
    }
}
