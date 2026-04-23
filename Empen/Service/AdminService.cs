using Empen.Data;
using Empen.Service.IService;
using ServerCore.Service;
using SharedData.Dto.Dummy;
using SharedData.Models;

namespace Empen.Service
{
    public class AdminService : IAdminService
    {
        private readonly PersonDbContext _personContext;
        private readonly IRedisService _redisService;

        public AdminService(PersonDbContext personContext, IRedisService redisService)
        {
            _personContext = personContext;
            _redisService = redisService;
        }

        // 로그인 데이터
        public async Task addDummyLoginAsync(DummyDataRequestDto request)
        {
            var list = new List<person_login>();
            DateTime now = DateTime.Now;

            for (int i = 0; i < request.Count; i++)
            {
                list.Add(new person_login
                {
                    person_id = request.PersonId,
                    insert_date = now,
                    update_date = now
                });
            }

            await _personContext.person_login.AddRangeAsync(list);
            await _personContext.SaveChangesAsync();
        }

        // 가챠 데이터
        public async Task addDummyGachaAsync(DummyDataRequestDto request)
        {
            var dummyGacha = new person_gacha
            {
                person_id = request.PersonId,
                gacha_id = request.GachaId,

                gacha_count = request.Count,

                insert_date = DateTime.Now,
                update_date = DateTime.Now
            };

            await _personContext.person_gacha.AddRangeAsync(dummyGacha);
            await _personContext.SaveChangesAsync();
        }

        public async Task clearCacheAsync(string targetCode)
        {
            if (targetCode == "ALL")
            {
                await _redisService.deleteKeysByPatternAsync("Master:*");
            }
            else
            {
                var listKey = CacheTargets.getRedisKey(targetCode);

                if (!string.IsNullOrEmpty(listKey))
                {
                    await _redisService.deleteAsync(listKey);

                    string patternKey = listKey.Replace(":All", ":*");
                    await _redisService.deleteKeysByPatternAsync(patternKey);
                }
            }

            await _redisService.publishAsync(CacheKey.Channel.Master, targetCode);
        }
    }
}
