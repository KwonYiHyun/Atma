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
    [Route("admin/reward")]
    [ApiController]
    [AdminApiKey]
    public class AdminRewardController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITimeService _timeService;
        private readonly IRedisService _redisService;

        public AdminRewardController(MasterDbContext masterContext, ITimeService timeService, IRedisService redisService)
        {
            _masterContext = masterContext;
            _timeService = timeService;
            _redisService = redisService;
        }

        // POST: admin/reward/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<List<MasterRewardDto>>>> getRewards()
        {
            var rewards = await _masterContext.master_reward
                .AsNoTracking()
                .Select(r => new MasterRewardDto
                {
                    reward_id = r.reward_id,
                    object_type = r.object_type,
                    object_value = r.object_value,
                    object_amount = r.object_amount,
                    additional_param = r.additional_param,
                    start_date = r.start_date,
                    end_date = r.end_date,
                    insert_date = r.insert_date,
                    update_date = r.update_date,
                })
                .ToListAsync();

            return Ok(new GameResponse<List<MasterRewardDto>>(rewards));
        }

        // POST: admin/reward/edit
        [HttpPost("edit")]
        public async Task<ActionResult<GameResponse<string>>> editReward([FromBody] MasterRewardDto rewardDto)
        {
            var reward = await _masterContext.master_reward.FirstOrDefaultAsync(r => r.reward_id == rewardDto.reward_id);
            if (reward == null)
            {
                return Ok(new GameResponse<string>(ErrorCode.DataNotFound, "리워드가 존재하지 않습니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            reward.object_type = rewardDto.object_type;
            reward.object_value = rewardDto.object_value;
            reward.object_amount = rewardDto.object_amount;
            reward.additional_param = rewardDto.additional_param;
            reward.start_date = rewardDto.start_date;
            reward.end_date = rewardDto.end_date;
            reward.update_date = now;

            try
            {
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.RewardAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "REWARD");
                return Ok(new GameResponse<string>("수정 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }

        // POST: admin/reward/add
        [HttpPost("add")]
        public async Task<ActionResult<GameResponse<string>>> addReward([FromBody] MasterRewardDto rewardDto)
        {
            bool exists = await _masterContext.master_reward.AnyAsync(r => r.reward_id == rewardDto.reward_id);
            if (exists)
            {
                return Ok(new GameResponse<string>(ErrorCode.DuplicationId, "이미 존재하는 ID입니다."));
            }

            DateTime now = await _timeService.getNowAsync();

            var newReward = new master_reward
            {
                reward_id = rewardDto.reward_id,
                object_type = rewardDto.object_type,
                object_value = rewardDto.object_value,
                object_amount = rewardDto.object_amount,
                additional_param = rewardDto.additional_param,
                start_date = rewardDto.start_date,
                end_date = rewardDto.end_date,
                insert_date = now,
                update_date = now
            };

            try
            {
                _masterContext.master_reward.Add(newReward);
                await _masterContext.SaveChangesAsync();
                await _redisService.deleteAsync(CacheKey.Master.RewardAll);
                await _redisService.publishAsync(CacheKey.Channel.Master, "REWARD");
                return Ok(new GameResponse<string>("추가 성공"));
            }
            catch (Exception ex)
            {
                return Ok(new GameResponse<string>(ErrorCode.ServerError, "서버 에러"));
            }
        }
    }
}
