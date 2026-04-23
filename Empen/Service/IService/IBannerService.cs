using SharedData.Dto;

namespace Empen.Service.IService
{
    public interface IBannerService
    {
        Task<ICollection<BannerInfoDto>> getAllBanner();
    }
}
