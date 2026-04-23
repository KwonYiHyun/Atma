using Empen.Filter;
using Empen.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Response;

namespace Empen.Controllers.Admin
{
    [Route("admin/person_cache")]
    [ApiController]
    [AdminApiKey]
    public class AdminPersonCacheController : ControllerBase
    {
        private readonly IPersonDataCacheService _personDataCacheService;

        public AdminPersonCacheController(IPersonDataCacheService personDataCacheService)
        {
            _personDataCacheService = personDataCacheService;
        }

        // POST: admin/person_cache/status/delete/{personId}
        [HttpPost("status/delete/{personId}")]
        public async Task<ActionResult<GameResponse<string>>> deletePersonStatusCache(int personId)
        {
            await _personDataCacheService.deletePersonStatusAsync(personId);

            return Ok(new GameResponse<string>("삭제 완료"));
        }

        // POST: admin/person_cache/item/delete/{personId}
        [HttpPost("item/delete/{personId}")]
        public async Task<ActionResult<GameResponse<string>>> deletePersonItemCache(int personId)
        {
            await _personDataCacheService.deletePersonItemAsync(personId);

            return Ok(new GameResponse<string>("삭제 완료"));
        }
    }
}
