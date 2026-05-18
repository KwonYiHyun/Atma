using Empen.Service;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Request;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers
{
    [Authorize]
    [Route("item")]
    [ApiController]
    public class ItemController : BaseController
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // POST: item/inventory
        [HttpPost("inventory")]
        public async Task<ActionResult<GameResponse<ICollection<CommonItemInfoDto>>>> getMyInventory()
        {
            var items = await _itemService.getAllItem(CurrentPersonId);

            return Ok(new GameResponse<ICollection<CommonItemInfoDto>>(items));
        }

        // POST: item/use
        [HttpPost("use")]
        public async Task<ActionResult<string>> useItem([FromBody] UseItemRequest query)
        {
            var result = await _itemService.useItemAndSaveAsync(CurrentPersonId, query.item_id, query.amount);

            if (result != ErrorCode.Success)
            {
                return Ok(new GameResponse<string>(result, "아이템 사용 실패"));
            }

            return Ok(new GameResponse<string>(result));
        }
    }
}
