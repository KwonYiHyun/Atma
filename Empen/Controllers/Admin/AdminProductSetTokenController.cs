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
    [Route("admin/product_set/token")]
    [ApiController]
    [AdminApiKey]
    public class AdminProductSetTokenController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminProductSetTokenController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/product_set/token/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterProductSetTokenDto>>>> getProductSetTokens()
        {
            var productSetTokens = await _masterContext.master_product_set_token
                .AsNoTracking()
                .Select(p => new MasterProductSetTokenDto
                {
                    product_set_token_id = p.product_set_token_id,
                    token_reward_group = p.token_reward_group,
                    show_order = p.show_order,
                    buy_upper_limit = p.buy_upper_limit,
                    price_token = p.price_token,
                    product_name = p.product_name,
                    product_detail = p.product_detail,
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

            return Ok(new GameResponse<List<MasterProductSetTokenDto>>(productSetTokens));
        }

        // POST: admin/product_set/token/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editProductSetToken([FromBody] MasterProductSetTokenDto productSetTokenDto)
        {
            var productSetToken = await _masterContext.master_product_set_token.FirstOrDefaultAsync(p => p.product_set_token_id == productSetTokenDto.product_set_token_id);
            if (productSetToken == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "패키지가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            productSetToken.token_reward_group = productSetTokenDto.token_reward_group;
            productSetToken.show_order = productSetTokenDto.show_order;
            productSetToken.buy_upper_limit = productSetTokenDto.buy_upper_limit;
            productSetToken.price_token = productSetTokenDto.price_token;
            productSetToken.product_name = productSetTokenDto.product_name;
            productSetToken.product_detail = productSetTokenDto.product_detail;
            productSetToken.image = productSetTokenDto.image;
            productSetToken.reward_id_1 = productSetTokenDto.reward_id_1;
            productSetToken.reward_id_2 = productSetTokenDto.reward_id_2;
            productSetToken.reward_id_3 = productSetTokenDto.reward_id_3;
            productSetToken.start_date = productSetTokenDto.start_date;
            productSetToken.end_date = productSetTokenDto.end_date;
            productSetToken.update_date = productSetTokenDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ProductSetTokenAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "PRODUCT_SET_TOKEN");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/product_set/token/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addProductSetToken([FromBody] MasterProductSetTokenDto productSetTokenDto)
        {
            bool exists = await _masterContext.master_product_set_token.AnyAsync(p => p.product_set_token_id == productSetTokenDto.product_set_token_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newProductSetToken = new master_product_set_token
            {
                product_set_token_id = productSetTokenDto.product_set_token_id,
                token_reward_group = productSetTokenDto.token_reward_group,
                show_order = productSetTokenDto.show_order,
                buy_upper_limit = productSetTokenDto.buy_upper_limit,
                price_token = productSetTokenDto.price_token,
                product_name = productSetTokenDto.product_name,
                product_detail = productSetTokenDto.product_detail,
                image = productSetTokenDto.image,
                reward_id_1 = productSetTokenDto.reward_id_1,
                reward_id_2 = productSetTokenDto.reward_id_2,
                reward_id_3 = productSetTokenDto.reward_id_3,
                start_date = productSetTokenDto.start_date,
                end_date = productSetTokenDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_product_set_token.Add(newProductSetToken);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ProductSetTokenAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "PRODUCT_SET_TOKEN");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
