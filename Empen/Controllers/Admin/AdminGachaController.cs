using Empen.Data;
using Empen.Filter;
using Empen.Service.IService;
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
    [Route("admin/gacha")]
    [ApiController]
    [AdminApiKey]
    public class AdminGachaController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminGachaController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/gacha/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterGachaDto>>>> getGachas()
        {
            var gachas = await _masterContext.master_gacha
                .AsNoTracking()
                .Select(g => new MasterGachaDto
                {
                    gacha_id = g.gacha_id,
                    gacha_name = g.gacha_name,
                    show_order = g.show_order,
                    gacha_type = g.gacha_type,
                    gacha_lot_group_id = g.gacha_lot_group_id,
                    gacha_consume_value = g.gacha_consume_value,
                    gacha_coupon_item_id = g.gacha_coupon_item_id,
                    gacha_image = g.gacha_image,
                    gacha_detail_image = g.gacha_detail_image,
                    gacha_detail_html = g.gacha_description,
                    gacha_point = g.gacha_point,
                    gacha_bonus_reward_group_lot_id = g.gacha_bonus_reward_group_lot_id,
                    start_date = g.start_date,
                    end_date = g.end_date,
                    insert_date = g.insert_date,
                    update_date = g.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterGachaDto>>(gachas));
        }

        // POST: admin/gacha/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editGacha([FromBody] MasterGachaDto gachaDto)
        {
            var gacha = await _masterContext.master_gacha.FirstOrDefaultAsync(g => g.gacha_id == gachaDto.gacha_id);
            if (gacha == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.ItemNotFound, "가챠가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            gacha.gacha_name = gachaDto.gacha_name;
            gacha.show_order = gachaDto.show_order;
            gacha.gacha_type = gachaDto.gacha_type;
            gacha.gacha_lot_group_id = gachaDto.gacha_lot_group_id;
            gacha.gacha_consume_value = gachaDto.gacha_consume_value;
            gacha.gacha_coupon_item_id = gachaDto.gacha_coupon_item_id;
            gacha.gacha_image = gachaDto.gacha_image;
            gacha.gacha_detail_image = gachaDto.gacha_detail_image;
            gacha.gacha_description = gachaDto.gacha_detail_html;
            gacha.gacha_point = gachaDto.gacha_point;
            gacha.gacha_bonus_reward_group_lot_id = gachaDto.gacha_bonus_reward_group_lot_id;
            gacha.gacha_exec_10_id = gachaDto.gacha_exec_10_id;
            gacha.start_date = gachaDto.start_date;
            gacha.end_date = gachaDto.end_date;
            gacha.update_date = now;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.GachaAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "GACHA");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception e)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/gacha/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addGacha([FromBody] MasterGachaDto gachaDto)
        {
            bool exists = await _masterContext.master_gacha.AnyAsync(g => g.gacha_id == gachaDto.gacha_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newGacha = new master_gacha
            {
                gacha_id = gachaDto.gacha_id,
                gacha_name = gachaDto.gacha_name,
                show_order = gachaDto.show_order,
                gacha_type = gachaDto.gacha_type,
                gacha_lot_group_id = gachaDto.gacha_lot_group_id,
                gacha_consume_value = gachaDto.gacha_consume_value,
                gacha_coupon_item_id = gachaDto.gacha_coupon_item_id,
                gacha_image = gachaDto.gacha_image,
                gacha_detail_image = gachaDto.gacha_detail_image,
                gacha_description = gachaDto.gacha_detail_html,
                gacha_point = gachaDto.gacha_point,
                gacha_bonus_reward_group_lot_id = gachaDto.gacha_bonus_reward_group_lot_id,
                gacha_exec_10_id = gachaDto.gacha_exec_10_id,
                start_date = gachaDto.start_date,
                end_date = gachaDto.end_date,
                update_date = now
            };

            try
            {
                _masterContext.master_gacha.Add(newGacha);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.GachaAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "GACHA");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
