using CloudStructures;
using CloudStructures.Structures;

namespace ServerCore.Service
{
    public class RedisService : IRedisService
    {
        private readonly RedisConnection _connection;
        // private readonly CacheMetricsService _metrics;

        // public RedisService(RedisConnection connection, CacheMetricsService metrics)
        public RedisService(RedisConnection connection)
        {
            _connection = connection;
            // _metrics = metrics;
        }

        public async Task<T?> getAsync<T>(string key)
        {
            var redis = new RedisString<T>(_connection, key, null);
            var result = await redis.GetAsync();

            if (result.HasValue)
            {
                // _metrics.recordHit();
                return result.Value;
            }

            // _metrics.recordMisses();
            return default;
        }

        public async Task<bool> setAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var redis = new RedisString<T>(_connection, key, null);
            return await redis.SetAsync(value, expiry);
        }

        public async Task<bool> deleteAsync(string key)
        {
            var redis = new RedisString<object>(_connection, key, null);
            return await redis.DeleteAsync();
        }

        public async Task<bool> existsAsync(string key)
        {
            var redis = new RedisString<object>(_connection, key, null);
            return await redis.ExistsAsync();
        }

        public async Task<long> incrementAsync(string key, long value = 1)
        {
            var redis = new RedisString<long>(_connection, key, null);
            return await redis.IncrementAsync(value);
        }

        public async Task<double> incrementFloatAsync(string key, double value)
        {
            var redis = new RedisString<double>(_connection, key, null);
            return await redis.IncrementAsync(value);
        }

        public async Task<bool> expireAsync(string key, TimeSpan expiry)
        {
            var redis = new RedisString<object>(_connection, key, null);
            return await redis.ExpireAsync(expiry);
        }

        public async Task<TimeSpan?> getTimeToLiveAsync(string key)
        {
            var redis = new RedisString<object>(_connection, key, null);
            return await redis.TimeToLiveAsync();
        }

        public async Task<long> listRightPushAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var redis = new RedisList<T>(_connection, key, null);
            var count = await redis.RightPushAsync(value);

            if (expiry.HasValue)
            {
                await redis.ExpireAsync(expiry.Value);
            }
            return count;
        }

        public async Task<List<T>> popAllListItemsAsync<T>(string key)
        {
            var redis = new RedisList<T>(_connection, key, null);
            var values = await redis.RangeAsync(0, -1);

            if (values.Length > 0)
            {
                await redis.DeleteAsync();
                return values.ToList();
            }

            return new List<T>();
        }

        public async Task<bool> setAddAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var redis = new RedisSet<T>(_connection, key, null);
            var isNew = await redis.AddAsync(value);

            if (expiry.HasValue)
            {
                await redis.ExpireAsync(expiry.Value);
            }
            return isNew;
        }

        public async Task<bool> setContainsAsync<T>(string key, T value)
        {
            var redis = new RedisSet<T>(_connection, key, null);
            return await redis.ContainsAsync(value);
        }

        public async Task<T> getOrSetAsync<T>(string key, Func<Task<T>> dbFetcher, TimeSpan? expiry = null)
        {
            var redis = new RedisString<T>(_connection, key, null);
            var cacheResult = await redis.GetAsync();

            if (cacheResult.HasValue)
            {
                // _metrics.recordHit();
                return cacheResult.Value;
            }

            // _metrics.recordMisses();
            var dbResult = await dbFetcher();

            if (dbResult != null)
            {
                await redis.SetAsync(dbResult, expiry ?? TimeSpan.FromHours(1));
            }

            return dbResult;
        }

        public async Task publishAsync(string channel, string message)
        {
            var sub = _connection.GetConnection().GetSubscriber();
            await sub.PublishAsync(channel, message);
        }

        public async Task subscribeAsync(string channel, Action<string> handler)
        {
            var sub = _connection.GetConnection().GetSubscriber();

            await sub.SubscribeAsync(channel, (redisChannel, redisValue) =>
            {
                handler(redisValue.ToString());
            });
        }

        public async Task deleteKeysByPatternAsync(string pattern)
        {
            var muxer = _connection.GetConnection();
            var db = muxer.GetDatabase();

            foreach (var endPoint in muxer.GetEndPoints())
            {
                var server = muxer.GetServer(endPoint);

                // server.Keys(): 내부적으로 SCAN을 사용하여 안전하게 키를 탐색
                await Task.Run(async () =>
                {
                    var keys = server.Keys(pattern: pattern).ToArray();

                    if (keys.Length > 0)
                    {
                        await db.KeyDeleteAsync(keys);
                    }
                });
            }
        }

        public async Task<bool> keyExistsAsync(string key)
        {
            var db = _connection.GetConnection().GetDatabase();
            return await db.KeyExistsAsync(key);
        }

        public async Task setStringAsync(string key, string value, TimeSpan? expiry)
        {
            var db = _connection.GetConnection().GetDatabase();
            await db.StringSetAsync(key, value, expiry);
        }

        public async Task<bool> setStringNxAsync(string key, string value, TimeSpan? expiry)
        {
            var db = _connection.GetConnection().GetDatabase();
            return await db.StringSetAsync(key, value, expiry, StackExchange.Redis.When.NotExists);
        }
    }
}
