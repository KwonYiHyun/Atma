namespace ServerCore.Service
{
    public interface IRedisLockService
    {
        Task<string?> lockAsync(string lockKey);
        Task<string?> lockAsync(string lockKey, int retryCount = 0, TimeSpan? retryDelay = null);
        Task<bool> unLockAsync(string lockKey, string uniqueValue);
    }
}
