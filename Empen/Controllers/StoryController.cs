using Empen.Service;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;

namespace Empen.Controllers
{
    [Authorize]
    [Route("story")]
    [ApiController]
    public class StoryController : BaseController
    {
        private readonly IStoryService _storyService;

        public StoryController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        // POST: story/script/{storyId}
        [HttpPost("script/{storyId}")]
        public async Task<ActionResult<ICollection<StoryScriptDto>>> getStoryScriptsByStoryId(int storyId)
        {
            var scripts = await _storyService.getStoryScriptsByStoryId(storyId);

            return Ok(scripts);
        }

        // POST: story/list/{groupId}
        [HttpPost("list/{groupId}")]
        public async Task<ActionResult<ICollection<StoryDto>>> getStorysByGroupId(int groupId)
        {
            var storys = await _storyService.getStorysByGroupId(groupId);

            return Ok(storys);
        }

        // POST: story/group
        [HttpPost("group")]
        public async Task<ActionResult<ICollection<StoryGroupDto>>> getAllStoryGroup()
        {
            var groups = await _storyService.getAllStoryGroup();

            return Ok(groups);
        }

        // POST: story/reward/{storyId}
        [HttpPost("reward/{storyId}")]
        public async Task<ActionResult<bool>> getStoryReward(int storyId)
        {
            var reward = await _storyService.getStoryReward(storyId, CurrentPersonId);

            return Ok(reward);
        }
    }
}
