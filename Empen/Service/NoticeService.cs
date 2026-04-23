using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto;

namespace Empen.Service
{
    public class NoticeService : INoticeService
    {
        //private readonly MasterDbContext _masterContext;
        private readonly IMasterDataCacheService _masterDataCacheService;
        private readonly ITimeService _timeService;

        public NoticeService(IMasterDataCacheService masterDataCacheService, ITimeService timeService)
        {
            _masterDataCacheService = masterDataCacheService;
            _timeService = timeService;
        }

        public async Task<ICollection<NoticeInfoDto>> getAllNotice()
        {
            DateTime now = await _timeService.getNowAsync();

            var noticeMasterAll = await _masterDataCacheService.getNoticeMapAsync();

            var notices = noticeMasterAll
                .Values
                .OrderBy(n => n.show_order)
                .Where(n => n.start_date <= now && (n.end_date > now || n.end_date == null))
                .Select(n => new NoticeInfoDto()
                {
                    notice_id = n.notice_id,
                    notice_link = n.notice_link,
                    notice_type = n.notice_type,
                    start_date = n.start_date,
                    end_date = n.end_date
                })
                .ToList();

            return notices;
        }
    }
}
