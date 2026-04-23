using Empen.Data;
using Empen.Filter;
using Empen.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto.Admin;
using SharedData.Models;
using SharedData.Request;
using SharedData.Response;
using SharedData.Type;
using static Empen.Data.CacheKey;

namespace Empen.Controllers.Admin
{
    [Route("admin/item")]
    [ApiController]
    [AdminApiKey]
    public class AdminItemController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly IItemService _itemService;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminItemController(MasterDbContext masterContext, IItemService itemService, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _itemService = itemService;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/item/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterItemDto>>>> getItems()
        {
            var items = await _masterContext.master_item
                .AsNoTracking()
                .Select(i => new MasterItemDto
                {
                    item_id = i.item_id,
                    show_order = i.show_order,
                    item_name = i.item_name,
                    item_type = i.item_type,
                    tab_type = i.tab_type,
                    item_param_1 = i.item_param_1,
                    item_param_2 = i.item_param_2,
                    item_description = i.item_description,
                    item_image_name = i.item_image_name,
                    expire_date = i.expire_date,
                    start_date = i.start_date,
                    end_date = i.end_date,
                    insert_date = i.insert_date,
                    update_date = i.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterItemDto>>(items));
        }

        // POST: admin/item/delete/{itemId}
        //[HttpPost("delete/{itemId}")]
        //public async Task<ActionResult<GameResponse<string>>> deleteItem(int itemId)
        //{
        //    var item = await _masterContext.master_item
        //        .FirstOrDefaultAsync(i => i.item_id == itemId);

        //    if (item == null)
        //    {
        //        return Ok(new GameResponse<string>(ErrorCode.ItemNotFound, "아이템이 존재하지 않습니다."));
        //    }

        //    _masterContext.Remove(item);
        //    await _masterContext.SaveChangesAsync();

        //    return Ok(new GameResponse<string>("성공"));
        //}

        // POST: admin/item/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editItem([FromBody] MasterItemDto itemDto)
        {
            var item = await _masterContext.master_item.FirstOrDefaultAsync(i => i.item_id == itemDto.item_id);
            if (item == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.ItemNotFound, "아이템이 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            item.show_order = itemDto.show_order;
            item.item_name = itemDto.item_name;
            item.item_type = itemDto.item_type;
            item.tab_type = itemDto.tab_type;
            item.item_param_1 = itemDto.item_param_1;
            item.item_param_2 = itemDto.item_param_2;
            item.item_description = itemDto.item_description;
            item.item_image_name = itemDto.item_image_name;
            item.expire_date = itemDto.expire_date;
            item.start_date = itemDto.start_date;
            item.end_date = itemDto.end_date;
            item.update_date = now;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ItemAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "ITEM");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/item/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addItem([FromBody] MasterItemDto itemDto)
        {
            bool exists = await _masterContext.master_item.AnyAsync(i => i.item_id == itemDto.item_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newItem = new master_item
            {
                item_id = itemDto.item_id,
                show_order = itemDto.show_order,
                item_name = itemDto.item_name,
                item_type = itemDto.item_type,
                tab_type = itemDto.tab_type,
                item_param_1 = itemDto.item_param_1,
                item_param_2 = itemDto.item_param_2,
                item_description = itemDto.item_description,
                item_image_name = itemDto.item_image_name,
                expire_date = itemDto.expire_date,
                start_date = itemDto.start_date,
                end_date = itemDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_item.Add(newItem);
                await _masterContext.SaveChangesAsync();

                await _redisService.deleteAsync(CacheKey.Master.ItemAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "ITEM");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        [HttpPost("admin/item/give")]
        public async Task<ActionResult<GameResponse<string>>> giveItem([FromBody] GiveItemRequest request)
        {
            var result = await _itemService.giveItemAndSaveAsync(request.person_id, request.item_id, request.amount);

            if (result != ErrorCode.Success)
            {
                return Ok(new GameResponse<string>(result));
            }

            return Ok(new GameResponse<string>("지급 완료"));
        }
    }
}
