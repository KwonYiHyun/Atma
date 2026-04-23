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
    [Route("admin/product")]
    [ApiController]
    [AdminApiKey]
    public class AdminProductController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminProductController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/product/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterProductDto>>>> getProducts()
        {
            var products = await _masterContext.master_product
                .AsNoTracking()
                .Select(p => new MasterProductDto
                {
                    product_id = p.product_id,
                    product_name = p.product_name,
                    prism = p.prism,
                    price = p.price,
                    product_type = p.product_type,
                    product_detail = p.product_detail,
                    start_date = p.start_date,
                    end_date = p.end_date,
                    insert_date = p.insert_date,
                    update_date = p.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterProductDto>>(products));
        }

        // POST: admin/product/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editProduct([FromBody] MasterProductDto productDto)
        {
            var product = await _masterContext.master_product.FirstOrDefaultAsync(p => p.product_id == productDto.product_id);
            if (product == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "등급 정보가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            product.product_name = productDto.product_name;
            product.prism = productDto.prism;
            product.price = productDto.price;
            product.product_type = productDto.product_type;
            product.product_detail = productDto.product_detail;
            product.start_date = productDto.start_date;
            product.end_date = productDto.end_date;
            product.update_date = productDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ProductAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "PRODUCT");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/product/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addProduct([FromBody] MasterProductDto productDto)
        {
            bool exists = await _masterContext.master_product.AnyAsync(p => p.product_id == productDto.product_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newProduct = new master_product
            {
                product_id = productDto.product_id,
                product_name = productDto.product_name,
                prism = productDto.prism,
                price = productDto.price,
                product_type = productDto.product_type,
                product_detail = productDto.product_detail,
                start_date = productDto.start_date,
                end_date = productDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_product.Add(newProduct);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ProductAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "PRODUCT");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
