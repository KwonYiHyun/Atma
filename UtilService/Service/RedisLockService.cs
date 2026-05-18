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

        public async Task<string?> lockAsync(string lockKey, int retryCount = 0, TimeSpan? retryDelay = null)
        {
            var db = _connection.GetConnection().GetDatabase();
            string key = _tokenKey + lockKey;
            string uniqueValue = Guid.NewGuid().ToString();

            int attempts = 0;
            while (attempts <= retryCount)
            {
                bool isAcquired = await db.LockTakeAsync(key, uniqueValue, _lockTime);
                if (isAcquired)
                {
                    return uniqueValue;
                }

                if (attempts < retryCount)
                {
                    attempts++;
                    await Task.Delay(retryDelay ?? TimeSpan.FromSeconds(1));
                }
                else
                {
                    break;
                }
            }

            return null;
        }

        public async Task<bool> unLockAsync(string lockKey, string uniqueValue)
        {
            var db = _connection.GetConnection().GetDatabase();
            string key = _tokenKey + lockKey;

            return await db.LockReleaseAsync(key, uniqueValue);
        }
    }
}
