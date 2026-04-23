using CloudStructures;

namespace ServerCore.Service
{
    public class RedisLockService : IRedisLockService
    {
        private readonly RedisConnection _connection;
        private readonly string _tokenKey = "Lock:";
        private readonly TimeSpan _lockTime = TimeSpan.FromSeconds(5);

        public RedisLockService(RedisConnection connection)
        {
            _connection = connection;
        }

        public async Task<string?> lockAsync(string lockKey)
        {
            var db = _connection.GetConnection().GetDatabase();
            string key = _tokenKey + lockKey;
            string uniqueValue = Guid.NewGuid().ToString();

            bool isAcquired = await db.LockTakeAsync(key, uniqueValue, _lockTime);

            return isAcquired ? uniqueValue : null;
        }

        public async Task<bool> unLockAsync(string lockKey, string uniqueValue)
        {
            var db = _connection.GetConnection().GetDatabase();
            string key = _tokenKey + lockKey;

            return await db.LockReleaseAsync(key, uniqueValue);
        }
    }
}
