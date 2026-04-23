using SharedData.Dto.Dummy;

namespace Empen.Service.IService
{
    public interface IAdminService
    {
        Task addDummyLoginAsync(DummyDataRequestDto request);
        Task addDummyGachaAsync(DummyDataRequestDto request);
        Task clearCacheAsync(string targetCode);
    }
}
