using SharedData.Dto;

namespace Empen.Service.IService
{
    public interface INoticeService
    {
        Task<ICollection<NoticeInfoDto>> getAllNotice();
    }
}
