namespace ServerCore.Service
{
    public class TimeService : ITimeService
    {
        private readonly IRedisService _redisService;
        private const string GLOBAL_OFFSET_KEY = "server:global_time_offset";

        public TimeService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<DateTime> getNowAsync()
        {
            var offsetTicksStr = await _redisService.getAsync<string>(GLOBAL_OFFSET_KEY);

            if (!string.IsNullOrEmpty(offsetTicksStr) && long.TryParse(offsetTicksStr, out long ticks))
            {
                return DateTime.Now.AddTicks(ticks);
            }

            return DateTime.Now;
        }
    }
}
