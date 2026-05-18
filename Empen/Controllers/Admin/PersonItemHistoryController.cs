using Empen.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto.Admin;
using SharedData.Response;

namespace Empen.Controllers.Admin
{
    [Route("admin/person_item_history")]
    [ApiController]
    [AdminApiKey]
    public class PersonItemHistoryController : ControllerBase
    {
        private readonly PersonDbContext _personContext;
        private readonly ITimeService _timeService;

        public PersonItemHistoryController(PersonDbContext personContext, ITimeService timeService)
        {
            _personContext = personContext;
            _timeService = timeService;
        }

        // POST: admin/person_item/get/{personId}
        [HttpPost("get/{personId}")]
        public async Task<ActionResult<GameResponse<List<PersonItemHistoryDto>>>> GetPersonItems(int personId)
        {
            var items = await _personContext.person_item_history
                .AsNoTracking()
                .Where(i => i.person_id == personId)
                .Select(i => new PersonItemHistoryDto
                {
                    person_item_history_id = i.person_item_history_id,
                    person_id = i.person_id,
                    item_id = i.item_id,
                    amount = i.amount,
                    insert_date = i.insert_date,
                    update_date = i.update_date
                })
                .ToListAsync();

            return Ok(new GameResponse<List<PersonItemHistoryDto>>(items));
        }
    }
}
