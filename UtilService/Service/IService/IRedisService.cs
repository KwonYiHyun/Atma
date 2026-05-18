namespace ServerCore.Service
{
    public interface IRedisService
    {
        Task<T?> getAsync<T>(string key);
        Task<bool> setAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> deleteAsync(string key);
        Task<bool> existsAsync(string key);
        Task<long> incrementAsync(string key, long value = 1);
        Task<double> incrementFloatAsync(string key, double value);
        Task<bool> expireAsync(string key, TimeSpan expiry);
        Task<TimeSpan?> getTimeToLiveAsync(string key);

        Task<long> listRightPushAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<List<T>> popAllListItemsAsync<T>(string key);

        Task<bool> setAddAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> setContainsAsync<T>(string key, T value);

        Task<T> getOrSetAsync<T>(string key, Func<Task<T>> dbFetcher, TimeSpan? expiry = null);

        Task publishAsync(string channel, string message);
        Task subscribeAsync(string channel, Action<string> handler);

        Task deleteKeysByPatternAsync(string pattern);

        Task<bool> keyExistsAsync(string key);
        Task setStringAsync(string key, string value, TimeSpan? expiry);
        Task<bool> setStringNxAsync(string key, string value, TimeSpan? expiry);
    }
}
