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
    [Route("admin/product_set/piece")]
    [ApiController]
    [AdminApiKey]
    public class AdminProductSetPieceController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminProductSetPieceController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/product_set/piece/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterProductSetPieceDto>>>> getProductSetPieces()
        {
            var productSets = await _masterContext.master_product_set_piece
                .AsNoTracking()
                .Select(p => new MasterProductSetPieceDto
                {
                    product_set_piece_id = p.product_set_piece_id,
                    show_order = p.show_order,
                    buy_upper_limit = p.buy_upper_limit,
                    cost_item_id = p.cost_item_id,
                    price_piece = p.price_piece,
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

            return Ok(new GameResponse<List<MasterProductSetPieceDto>>(productSets));
        }

        // POST: admin/product_set/piece/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editProductSetPiece([FromBody] MasterProductSetPieceDto productSetPieceDto)
        {
            var productSetPiece = await _masterContext.master_product_set_piece.FirstOrDefaultAsync(p => p.product_set_piece_id == productSetPieceDto.product_set_piece_id);
            if (productSetPiece == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "패키지가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            productSetPiece.show_order = productSetPieceDto.show_order;
            productSetPiece.buy_upper_limit = productSetPieceDto.buy_upper_limit;
            productSetPiece.cost_item_id = productSetPieceDto.cost_item_id;
            productSetPiece.price_piece = productSetPieceDto.price_piece;
            productSetPiece.product_name = productSetPieceDto.product_name;
            productSetPiece.product_detail = productSetPieceDto.product_detail;
            productSetPiece.image = productSetPieceDto.image;
            productSetPiece.reward_id_1 = productSetPieceDto.reward_id_1;
            productSetPiece.reward_id_2 = productSetPieceDto.reward_id_2;
            productSetPiece.reward_id_3 = productSetPieceDto.reward_id_3;
            productSetPiece.start_date = productSetPieceDto.start_date;
            productSetPiece.end_date = productSetPieceDto.end_date;
            productSetPiece.update_date = productSetPieceDto.update_date;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ProductSetPieceAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "PRODUCT_SET_PIECE");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/product_set/piece/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addProductSetPiece([FromBody] MasterProductSetPieceDto productSetPieceDto)
        {
            bool exists = await _masterContext.master_product_set_piece.AnyAsync(p => p.product_set_piece_id == productSetPieceDto.product_set_piece_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newProductSetPiece = new master_product_set_piece
            {
                product_set_piece_id = productSetPieceDto.product_set_piece_id,
                show_order = productSetPieceDto.show_order,
                buy_upper_limit = productSetPieceDto.buy_upper_limit,
                cost_item_id = productSetPieceDto.cost_item_id,
                price_piece = productSetPieceDto.price_piece,
                product_name = productSetPieceDto.product_name,
                product_detail = productSetPieceDto.product_detail,
                image = productSetPieceDto.image,
                reward_id_1 = productSetPieceDto.reward_id_1,
                reward_id_2 = productSetPieceDto.reward_id_2,
                reward_id_3 = productSetPieceDto.reward_id_3,
                start_date = productSetPieceDto.start_date,
                end_date = productSetPieceDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_product_set_piece.Add(newProductSetPiece);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.ProductSetPieceAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "PRODUCT_SET_PIECE");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
