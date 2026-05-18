using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;

namespace Empen.Service
{
    public class PersonDataCacheService : IPersonDataCacheService
    {
        private readonly IDbContextFactory<PersonDbContext> _personFactory;
        private readonly IRedisService _redisService;
        private readonly IRedisLockService _redisLockService;

        private readonly TimeSpan _defaultRedisExpiry = TimeSpan.FromMinutes(5);

        public PersonDataCacheService(IDbContextFactory<PersonDbContext> personFactory, IRedisService redisService, IRedisLockService redisLockService)
        {
            _personFactory = personFactory;
            _redisLockService = redisLockService;
            _redisService = redisService;
        }

        public async Task<List<PersonItem>> getPersonItemAsync(int personId)
        {
            return await getOrSetAsync(CacheKey.Person.PersonItem(personId), async () =>
            {
                using var context = _personFactory.CreateDbContext();
                return await context.person_item
                    .Where(i => i.person_id == personId && i.amount > 0)
                    .Select(i => new PersonItem
                    {
                        person_item_id = i.person_item_id,
                        item_id = i.item_id,
                        amount = i.amount
                    })
                    .AsNoTracking()
                    .ToListAsync();
            });
        }

        public async Task deletePersonItemAsync(int personId)
        {
            await _redisService.deleteAsync(CacheKey.Person.PersonItem(personId));
        }

        public async Task<PersonStatus> getPersonStatusAsync(int personId)
        {
            return await getOrSetAsync(CacheKey.Person.PersonStatus(personId), async () =>
            {
                using var context = _personFactory.CreateDbContext();

                return await context.person_status
                    .Where(s => s.person_id == personId)
                    .Select(s => new PersonStatus
                    {
                        display_person_id = s.display_person_id,
                        person_hash = s.person_hash,
                        email = s.email,
                        person_name = s.person_name,
                        level = s.level,
                        exp = s.exp,
                        token = s.token,
                        gift = s.gift,
                        manual = s.manual,
                        film = s.film,
                        prism = s.prism,
                        leader_person_character_id = s.leader_person_character_id,
                        introduce = s.introduce,
                        insert_date = s.insert_date
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            });
        }

        public async Task deletePersonStatusAsync(int personId)
        {
            await _redisService.deleteAsync(CacheKey.Person.PersonStatus(personId));
        }

        private async Task<T> getOrSetAsync<T>(
            string redisKey,
            Func<Task<T>> dbFetcher,
            TimeSpan? redisExpiry = null) where T : class
        {
            var data = await _redisService.getAsync<T>(redisKey);
            if (data != null)
            {
                return data;
            }

            string lockKey = "Lock:" + redisKey;
            string? token = await _redisLockService.lockAsync(lockKey);

            if (token != null)
            {
                try
                {
                    data = await _redisService.getAsync<T>(redisKey);
                    if (data != null)
                    {
                        return data;
                    }

                    data = await dbFetcher();

                    if (data != null)
                    {
                        await _redisService.setAsync(redisKey, data, redisExpiry ?? _defaultRedisExpiry);
                    }
                }
                finally
                {
                    await _redisLockService.unLockAsync(lockKey, token);
                }
            }
            else
            {
                await Task.Delay(300);
                data = await _redisService.getAsync<T>(redisKey);
            }

            return data;
        }
    }
}
