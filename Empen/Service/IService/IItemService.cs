using SharedData.Dto;
using SharedData.Type;

namespace Empen.Service.IService
{
    public interface IItemService
    {
        Task<ICollection<CommonItemInfoDto>> getAllItem(int personId);
        Task<ErrorCode> giveItemAsync(int personId, int itemId, int count);
        Task<ErrorCode> giveItemAndSaveAsync(int personId, int itemId, int amount);
        Task<ErrorCode> useItemAsync(int personId, int itemId, int amount);
        Task<ErrorCode> useItemAndSaveAsync(int personId, int itemId, int count);
        Task commitAsync();
    }
}
