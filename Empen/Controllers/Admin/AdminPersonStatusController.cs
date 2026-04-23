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

        public AdminPersonStatusController(PersonDbContext personContext, ITimeService timeService, IPersonDataCacheService personDataCacheService)
        {
            _personContext = personContext;
            _timeService = timeService;
            _personDataCacheService = personDataCacheService;
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
            var now = await _timeService.getNowAsync();

            var updatedRows = await _personContext.person_status
                .Where(p => p.person_status_id == dto.person_status_id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.person_name, dto.person_name)
                    .SetProperty(p => p.level, dto.level)
                    .SetProperty(p => p.exp, dto.exp)
                    .SetProperty(p => p.token, dto.token)
                    .SetProperty(p => p.gift, dto.gift)
                    .SetProperty(p => p.manual, dto.manual)
                    .SetProperty(p => p.film, dto.flim)
                    .SetProperty(p => p.prism, dto.prism)
                    .SetProperty(p => p.introduce, dto.introduce)
                    .SetProperty(p => p.update_date, now)
                );

            if (updatedRows == 0)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정할 데이터가 없습니다."));
            }
            await _personDataCacheService.deletePersonStatusAsync(dto.person_id);
            return Ok(new GameResponse<string>("수정 성공"));
        }
    }
}