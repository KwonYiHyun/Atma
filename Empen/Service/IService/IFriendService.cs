using SharedData.Dto;
using SharedData.Type;

namespace Empen.Service.IService
{
    public interface IFriendService
    {
        // 내가 추가한 사람
        Task<ICollection<FriendInfoDto>> getAllFollowingList(int personId);

        // 나를 추가한 사람
        Task<ICollection<FriendInfoDto>> getAllFollowerList(int personId);

        Task<ICollection<FriendInfoDto>> getRecommendList(int myId);

        // 팔로잉 요청
        Task<ErrorCode> requestFollowing(int personId, int friendId);
        Task<ErrorCode> requestFollowingById(int personId, int displayId);

        // 내가 팔로잉 하고있는 사람 삭제
        Task<ErrorCode> deleteFollowing(int personId, int friendId);

        // 나를 팔로잉 하고있는 사람 삭제
        Task<ErrorCode> deleteFollower(int personId, int friendId);
    }
}
