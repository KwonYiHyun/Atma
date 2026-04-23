using Empen.Service;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers
{
    [Authorize]
    [Route("friend")]
    [ApiController]
    public class FriendController : BaseController
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        // POST: friend/following
        [HttpPost("following")]
        public async Task<ActionResult<GameResponse<ICollection<FriendInfoDto>>>> getAllFollowing()
        {
            var following = await _friendService.getAllFollowingList(CurrentPersonId);

            return Ok(new GameResponse<ICollection<FriendInfoDto>>(following));
        }

        // POST: friend/follower
        [HttpPost("follower")]
        public async Task<ActionResult<GameResponse<ICollection<FriendInfoDto>>>> getAllFollower()
        {
            var follower = await _friendService.getAllFollowerList(CurrentPersonId);

            return Ok(new GameResponse<ICollection<FriendInfoDto>>(follower));
        }

        // POST: friend/request/friendId/{friendId}
        [HttpPost("request/friendId/{friendId}")]
        public async Task<ActionResult<ErrorCode>> requestFriendByFriendId(int friendId)
        {
            var request = await _friendService.requestFollowing(CurrentPersonId, friendId);

            return Ok(request);
        }

        // POST: friend/request/displayId/{displayId}
        [HttpPost("request/displayId/{displayId}")]
        public async Task<ActionResult<ErrorCode>> requestFriendByDisplayId(int displayId)
        {
            var request = await _friendService.requestFollowingById(CurrentPersonId, displayId);

            return Ok(request);
        }

        // POST: friend/recommendlist
        [HttpPost("recommendlist")]
        public async Task<ActionResult<ICollection<FriendInfoDto>>> getRecommendList()
        {
            var list = await _friendService.getRecommendList(CurrentPersonId);

            return Ok(list);
        }

        // POST: friend/delete/following/{friendId}
        [HttpPost("delete/following/{friendId}")]
        public async Task<ActionResult<ErrorCode>> deleteFollowing(int friendId)
        {
            var result = await _friendService.deleteFollowing(CurrentPersonId, friendId);

            return Ok(result);
        }

        // POST: friend/delete/follower/{friendId}
        [HttpPost("delete/follower/{friendId}")]
        public async Task<ActionResult<ErrorCode>> deleteFollower(int friendId)
        {
            var result = await _friendService.deleteFollower(CurrentPersonId, friendId);

            return Ok(result);
        }
    }
}
