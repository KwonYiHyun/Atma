using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;

namespace Empen.Service.IService
{
    public interface IRewardService
    {
        Task<Dictionary<int, ObjectDisplayDto>> getObjectDisplayAsync(IEnumerable<int> rewardIds);
        Task<List<ObjectDisplayDto>> getObjectDisplayListByContentAsync(Dictionary<int, int> itemAmounts, List<int> characterIds, int prism, int token, int gacha);
        Task<ObjectDisplayDto?> getObjectDisplayOneAsync(int rewardId);
        public ObjectDisplayDto createObjectDisplayByItem(master_item item, int amount);
        public ObjectDisplayDto createObjectDisplayByCharacter(master_character charData);
    }
}
