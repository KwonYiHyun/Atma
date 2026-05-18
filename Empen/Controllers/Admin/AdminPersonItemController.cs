using Empen.Filter;
using Empen.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto.Admin;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers.Admin
{
    [Route("admin/person_item")]
    [ApiController]
    [AdminApiKey]
    public class AdminPersonItemController : ControllerBase
    {
        private readonly PersonDbContext _personContext;
        private readonly ITimeService _timeService;
        private readonly IPersonDataCacheService _personDataCacheService;
        private readonly IRedisLockService _redisLockService;
        private readonly ILogger<AdminPersonItemController> _logger;

        public AdminPersonItemController(PersonDbContext personContext, ITimeService timeService, IPersonDataCacheService personDataCacheService, IRedisLockService redisLockService, ILogger<AdminPersonItemController> logger)
        {
            _personContext = personContext;
            _timeService = timeService;
            _personDataCacheService = personDataCacheService;
            _redisLockService = redisLockService;
            _logger = logger;
        }

        // POST: admin/person_item/get/{personId}
        [HttpPost("get/{personId}")]
        public async Task<ActionResult<GameResponse<List<PersonItemDto>>>> GetPersonItems(int personId)
        {
            var items = await _personContext.person_item
                .AsNoTracking()
                .Where(i => i.person_id == personId)
                .Select(i => new PersonItemDto
                {
                    person_item_id = i.person_item_id,
                    person_id = i.person_id,
                    item_id = i.item_id,
                    amount = i.amount,
                    insert_date = i.insert_date,
                    update_date = i.update_date
                })
                .ToListAsync();

            return Ok(new GameResponse<List<PersonItemDto>>(items));
        }

        // POST: admin/person_item/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> EditPersonItem([FromBody] PersonItemDto dto)
        {
            var itemOwner = await _personContext.person_item
                .AsNoTracking()
                .Where(i => i.person_item_id == dto.person_item_id)
                .Select(i => new { i.person_id })
                .FirstOrDefaultAsync();

            if (itemOwner == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "아이템이 존재하지 않습니다."));
            }

            // 분산락
            string lockKey = itemOwner.person_id.ToString();
            string? lockToken = await _redisLockService.lockAsync(lockKey, 3, TimeSpan.FromSeconds(1));

            if (lockToken == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.UserDataLocked, "유저 데이터가 사용 중입니다. 잠시 후 다시 시도해주세요."));
            }

            try
            {
                // 분산락 잡은 후 데이터가 최신인지 확인
                var latestItem = await _personContext.person_item
                    .Where(i => i.person_item_id == dto.person_item_id)
                    .FirstOrDefaultAsync();

                if (latestItem == null)
                {
                    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정 직전 아이템이 삭제되었습니다."));
                }

                if (latestItem.update_date != dto.update_date)
                {
                    return Ok(new GameResponse<string>(ErrorCode.AlreadyProcessed, "데이터가 변경되었습니다. 다시 조회 후 시도해주세요."));
                }

                DateTime now = await _timeService.getNowAsync();
                latestItem.amount = dto.amount;
                latestItem.update_date = now;

                await _personContext.SaveChangesAsync();
                await _personDataCacheService.deletePersonItemAsync(itemOwner.person_id);

                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"EditPersonItem Exception! ItemId: {dto.person_item_id}");
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "수정 중 오류가 발생했습니다."));
            }
            finally
            {
                await _redisLockService.unLockAsync(lockKey, lockToken);
            }
        }

        // POST: admin/person_item/delete
        [HttpPost("delete")]
        public async Task<ActionResult<GameResponse<string>>> DeletePersonItem([FromBody] PersonItemDeleteDto dto)
        {
            var itemOwner = await _personContext.person_item
                .AsNoTracking()
                .Where(i => i.person_item_id == dto.person_item_id)
                .Select(i => new { i.person_id })
                .FirstOrDefaultAsync();

            if (itemOwner == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "삭제할 아이템이 없습니다."));
            }

            // 분산락
            string lockKey = itemOwner.person_id.ToString();
            string? lockToken = await _redisLockService.lockAsync(lockKey, 3, TimeSpan.FromSeconds(1));

            if (lockToken == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.UserDataLocked, "유저 데이터가 사용 중이라 삭제할 수 없습니다."));
            }

            try
            {
                var targetItem = await _personContext.person_item
                    .Where(i => i.person_item_id == dto.person_item_id)
                    .FirstOrDefaultAsync();

                if (targetItem == null)
                {
                    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "이미 데이터가 삭제되었습니다."));
                }

                if (dto.update_date != targetItem.update_date)
                {
                    return Ok(new GameResponse<string>(ErrorCode.AlreadyProcessed, "데이터가 변경되었습니다. 다시 조회 후 시도해주세요."));
                }

                _personContext.person_item.Remove(targetItem);
                await _personContext.SaveChangesAsync();
                await _personDataCacheService.deletePersonItemAsync(itemOwner.person_id);
                _logger.LogInformation($"DeletePersonItem PersonId: {itemOwner.person_id} ItemId: {targetItem.item_id} Amount: {targetItem.amount}");
                return Ok(new GameResponse<string>("삭제 성공"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeletePersonItem Exception! ItemId: {dto.person_item_id}");
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "삭제 중 오류가 발생했습니다."));
            }
            finally
            {
                await _redisLockService.unLockAsync(lockKey, lockToken);
            }


            //int personId = await _personContext.person_item
            //    .AsNoTracking()
            //    .Where(i => i.person_item_id == personItemId)
            //    .Select(i => i.person_id)
            //    .FirstOrDefaultAsync();

            //var deletedRows = await _personContext.person_item
            //    .Where(i => i.person_item_id == personItemId)
            //    .ExecuteDeleteAsync();

            //if (deletedRows == 0)
            //{
            //    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "삭제할 데이터가 없습니다."));
            //}
            //await _personDataCacheService.deletePersonItemAsync(personId);
            //return Ok(new GameResponse<string>("삭제 성공"));
        }
    }
}