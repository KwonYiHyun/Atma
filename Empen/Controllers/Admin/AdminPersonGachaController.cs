using Empen.Data;
using Empen.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto.Admin;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers.Admin
{
    [Route("admin/person_gacha")]
    [ApiController]
    [AdminApiKey]
    public class AdminPersonGachaController : ControllerBase
    {
        private readonly PersonDbContext _personContext;
        private readonly ITimeService _timeService;

        public AdminPersonGachaController(PersonDbContext personContext, ITimeService timeService)
        {
            _personContext = personContext;
            _timeService = timeService;
        }

        // POST: admin/person_gacha/get/{personId}
        [HttpPost("get/{personId}")]
        public async Task<ActionResult<GameResponse<List<PersonGachaDto>>>> GetPersonGachas(int personId)
        {
            var gachas = await _personContext.person_gacha
                .AsNoTracking()
                .Where(g => g.person_id == personId)
                .Select(g => new PersonGachaDto
                {
                    person_gacha_id = g.person_gacha_id,
                    person_id = g.person_id,
                    gacha_id = g.gacha_id,
                    gacha_count = g.gacha_count,
                    insert_date = g.insert_date,
                    update_date = g.update_date
                })
                .ToListAsync();

            return Ok(new GameResponse<List<PersonGachaDto>>(gachas));
        }

        // POST: admin/person_gacha/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> EditPersonGacha([FromBody] PersonGachaDto dto)
        {
            DateTime now = await _timeService.getNowAsync();

            var updatedRows = await _personContext.person_gacha
                .Where(g => g.person_gacha_id == dto.person_gacha_id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.gacha_count, dto.gacha_count)
                    .SetProperty(p => p.update_date, now)
                );

            if (updatedRows == 0)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "수정할 가챠 기록이 존재하지 않습니다."));
            }

            return Ok(new GameResponse<string>("수정 성공"));
        }

        // POST: admin/person_gacha/delete/{personGachaId}
        [HttpPost("delete/{personGachaId}")]
        public async Task<ActionResult<GameResponse<string>>> DeletePersonGacha(int personGachaId)
        {
            var deletedRows = await _personContext.person_gacha
                .Where(g => g.person_gacha_id == personGachaId)
                .ExecuteDeleteAsync();

            if (deletedRows == 0)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "삭제할 가챠 기록이 존재하지 않습니다."));
            }

            return Ok(new GameResponse<string>("삭제 성공"));
        }
    }
}