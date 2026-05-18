using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Response;

namespace Empen.Controllers
{
    [Authorize]
    [Route("banner")]
    [ApiController]
    public class BannerController : BaseController
    {
        private readonly IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        // POST: banner/all
        [HttpPost("all")]
        public async Task<ActionResult<GameResponse<ICollection<BannerInfoDto>>>> getAllBanner()
        {
            var banners = await _bannerService.getAllBanner();

            return Ok(new GameResponse<ICollection<BannerInfoDto>>(banners));
        }
    }
}
