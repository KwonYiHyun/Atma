using SharedData.Dto;

namespace Empen.Service.IService
{
    public interface IStoryService
    {
        Task<ICollection<StoryGroupDto>> getAllStoryGroup();
        Task<ICollection<StoryDto>> getStorysByGroupId(int storyGroupId);
        Task<ICollection<StoryScriptDto>> getStoryScriptsByStoryId(int storyId);
        Task<bool> getStoryReward(int storyId, int personId);
    }
}
