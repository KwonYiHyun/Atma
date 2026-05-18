using SharedData.Dto;
using SharedData.Type;

namespace Empen.Service.IService
{
    public interface IAchievementService
    {
        Task<ICollection<AchievementDto>> getAllAchievementAsync(int personId);
        Task<(ErrorCode, ICollection<ObjectDisplayDto>)> claimAchievementAsync(int personId, int achievementId);
        Task<(ErrorCode, ICollection<ObjectDisplayDto>)> claimAllAchievementAsync(int personId);
        Task checkAndSetNewFlagAsync(int personId, AchievementType? targetType = null);
    }
}
