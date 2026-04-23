namespace ServerCore.Service
{
    public interface IRedisLockService
    {
        Task<string?> lockAsync(string lockKey);
        Task<bool> unLockAsync(string lockKey, string uniqueValue);
    }
}
