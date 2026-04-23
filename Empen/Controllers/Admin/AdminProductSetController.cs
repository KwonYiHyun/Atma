using Empen.Data;
using Empen.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto.Admin;
using SharedData.Models;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers.Admin
{
    [Route("admin/product_set")]
    [ApiController]
    [AdminApiKey]
    public class AdminProductSetController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminProductSetController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/product_set/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterProductSetDto>>>> getProductSets()
        {
            var productSets = await _masterContext.master_product_set
                .AsNoTracking()
                .Select(p => new MasterProductSetDto
                {
                    product_set_id = p.product_set_id,
                    product_id = p.product_id,
                    show_order = p.show_order,
                    buy_upper_limit = p.buy_upper_limit,
                    image = p.image,
                    reward_id_1 = p.reward_id_1,
                    reward_id_2 = p.reward_id_2,
                    reward_id_3 = p.reward_id_3,
                    start_date = p.start_date,
                    end_date = p.end_date,
                    insert_date = p.insert_date,
                    update_date = p.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterProductSetDto>>(productSets));
        }

        // POST: admin/product_set/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editProductSet([FromBody] MasterProductSetDto productSetDto)
        {
            var productSet = await _masterContext.master_product_set.FirstOrDefaultAsync(p => p.product_set_id == productSetDto.product_set_id);
            if (productSet == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "패키지가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            productSet.product_id = productSetDto.product_id;
            productSet.show_order = productSetDto.show_order;
            productSet.buy_upper_limit = productSetDto.buy_upper_limit;
            productSet.image = productSetDto.image;
            productSet.reward_id_1 = productSetDto.reward_id_1;
            productSet.reward_id_2 = productSetDto.reward_id_2;
            productSet.reward_id_3 = productSetDto.reward_id_3;
            productSet.start_date = productSetDto.start_date;
            productSet.end_date = productSetDto.end_date;
            productSet.update_date = productSetDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ProductSetAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "PRODUCT_SET");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/product_set/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addProductSet([FromBody] MasterProductSetDto productSetDto)
        {
            bool exists = await _masterContext.master_product_set.AnyAsync(p => p.product_set_id == productSetDto.product_set_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newProductSet = new master_product_set
            {
                product_set_id = productSetDto.product_set_id,
                product_id = productSetDto.product_id,
                show_order = productSetDto.show_order,
                buy_upper_limit = productSetDto.buy_upper_limit,
                image = productSetDto.image,
                reward_id_1 = productSetDto.reward_id_1,
                reward_id_2 = productSetDto.reward_id_2,
                reward_id_3 = productSetDto.reward_id_3,
                start_date = productSetDto.start_date,
                end_date = productSetDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_product_set.Add(newProductSet);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ProductSetAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "PRODUCT_SET");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
