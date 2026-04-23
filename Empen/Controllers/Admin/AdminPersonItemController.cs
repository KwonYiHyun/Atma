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

        public AdminPersonItemController(PersonDbContext personContext, ITimeService timeService, IPersonDataCacheService personDataCacheService)
        {
            _personContext = personContext;
            _timeService = timeService;
            _personDataCacheService = personDataCacheService;
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
            DateTime now = await _timeService.getNowAsync();

            var updatedRows = await _personContext.person_item
                .Where(i => i.person_item_id == dto.person_item_id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.amount, dto.amount)
                    .SetProperty(p => p.update_date, now)
                );

            if (updatedRows == 0)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "아이템이 존재하지 않습니다."));
            }
            await _personDataCacheService.deletePersonItemAsync(dto.person_id);
            return Ok(new GameResponse<string>("수정 성공"));
        }

        // POST: admin/person_item/delete/{personItemId}
        [HttpPost("delete/{personItemId}")]
        public async Task<ActionResult<GameResponse<string>>> DeletePersonItem(int personItemId)
        {
            int personId = await _personContext.person_item
                .AsNoTracking()
                .Where(i => i.person_item_id == personItemId)
                .Select(i => i.person_id)
                .FirstOrDefaultAsync();

            var deletedRows = await _personContext.person_item
                .Where(i => i.person_item_id == personItemId)
                .ExecuteDeleteAsync();

            if (deletedRows == 0)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "삭제할 데이터가 없습니다."));
            }
            await _personDataCacheService.deletePersonItemAsync(personId);
            return Ok(new GameResponse<string>("삭제 성공"));
        }
    }
}