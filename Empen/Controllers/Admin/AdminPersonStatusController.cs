using Empen.Data;
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
    [Route("admin/person_status")]
    [ApiController]
    [AdminApiKey]
    public class AdminPersonStatusController : ControllerBase
    {
        private readonly PersonDbContext _personContext;
        private readonly ITimeService _timeService;
        private readonly IPersonDataCacheService _personDataCacheService;
        private readonly IRedisLockService _redisLockService;
        private readonly ILogger<AdminPersonStatusController> _logger;

        public AdminPersonStatusController(PersonDbContext personContext, ITimeService timeService, IPersonDataCacheService personDataCacheService, IRedisLockService redisLockService, ILogger<AdminPersonStatusController> logger)
        {
            _personContext = personContext;
            _timeService = timeService;
            _personDataCacheService = personDataCacheService;
            _redisLockService = redisLockService;
            _logger = logger;
        }

        // POST: admin/person_status/get/{personId}
        [HttpPost("get/{personId}")]
        public async Task<ActionResult<GameResponse<PersonStatusDto>>> getPersonStatus(int personId)
        {
            var status = await _personContext.person_status
                .AsNoTracking()
                .Where(p => p.person_id == personId)
                .Select(p => new PersonStatusDto
                {
                    person_status_id = p.person_status_id,
                    person_id = p.person_id,
                    display_person_id = p.display_person_id,
                    person_hash = p.person_hash,
                    email = p.email,
                    person_name = p.person_name,
                    level = p.level,
                    exp = p.exp,
                    token = p.token,
                    gift = p.gift,
                    manual = p.manual,
                    flim = p.film,
                    prism = p.prism,
                    character_amount_max = p.character_amount_max,
                    character_storage_amount_max = p.character_storage_amount_max,
                    leader_person_character_id = p.leader_person_character_id,
                    introduce = p.introduce,
                    insert_date = p.insert_date,
                    update_date = p.update_date
                })
                .FirstOrDefaultAsync();

            if (status == null)
            {
                return Ok(new GameResponse<PersonStatusDto>(ErrorCode.DataNotFound, "유저를 찾을 수 없습니다."));
            }
            await _personDataCacheService.deletePersonStatusAsync(personId);
            return Ok(new GameResponse<PersonStatusDto>(status));
        }

        // POST: admin/person_status/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editPersonStatus([FromBody] PersonStatusDto dto)
        {
            var ownerInfo = await _personContext.person_status
                .AsNoTracking()
                .Where(p => p.person_status_id == dto.person_status_id)
                .Select(p => new { p.person_id })
                .FirstOrDefaultAsync();

            if (ownerInfo == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정할 유저 데이터가 없습니다."));
            }

            // 분산락
            string lockKey = ownerInfo.person_id.ToString();
            string? lockToken = await _redisLockService.lockAsync(lockKey, 3, TimeSpan.FromSeconds(1));

            if (lockToken == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.UserDataLocked, "유저가 현재 게임 이용 중입니다. 잠시 후 다시 시도해주세요."));
            }

            try
            {
                // 분산락 잡은 후 데이터가 최신인지 확인
                var latestStatus = await _personContext.person_status
                    .Where(p => p.person_status_id == dto.person_status_id)
                    .FirstOrDefaultAsync();

                if (latestStatus == null)
                {
                    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정 직전 유저 데이터가 삭제되었습니다."));
                }

                var now = await _timeService.getNowAsync();

                latestStatus.person_name = dto.person_name;
                latestStatus.level = dto.level;
                latestStatus.exp = dto.exp;
                latestStatus.token = dto.token;
                latestStatus.gift = dto.gift;
                latestStatus.manual = dto.manual;
                latestStatus.film = dto.flim;
                latestStatus.prism = dto.prism;
                latestStatus.introduce = dto.introduce;
                latestStatus.update_date = now;

                await _personContext.SaveChangesAsync();
                await _personDataCacheService.deletePersonStatusAsync(ownerInfo.person_id);

                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"editPersonStatus Exception! PersonId: {ownerInfo.person_id}");
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "수정 중 오류가 발생했습니다."));
            }
            finally
            {
                await _redisLockService.unLockAsync(lockKey, lockToken);
            }


            //var now = await _timeService.getNowAsync();

            //var updatedRows = await _personContext.person_status
            //    .Where(p => p.person_status_id == dto.person_status_id)
            //    .ExecuteUpdateAsync(s => s
            //        .SetProperty(p => p.person_name, dto.person_name)
            //        .SetProperty(p => p.level, dto.level)
            //        .SetProperty(p => p.exp, dto.exp)
            //        .SetProperty(p => p.token, dto.token)
            //        .SetProperty(p => p.gift, dto.gift)
            //        .SetProperty(p => p.manual, dto.manual)
            //        .SetProperty(p => p.film, dto.flim)
            //        .SetProperty(p => p.prism, dto.prism)
            //        .SetProperty(p => p.introduce, dto.introduce)
            //        .SetProperty(p => p.update_date, now)
            //    );

            //if (updatedRows == 0)
            //{
            //    return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정할 데이터가 없습니다."));
            //}
            //await _personDataCacheService.deletePersonStatusAsync(dto.person_id);
            //return Ok(new GameResponse<string>("수정 성공"));
        }
    }
}