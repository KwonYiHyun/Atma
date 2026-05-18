namespace Empen.Service
{
    public class CacheMetricsService
    {
        private readonly ILogger<CacheMetricsService> _logger;
        private long _cacheHits;
        private long _cacheMisses;

        public CacheMetricsService(ILogger<CacheMetricsService> logger)
        {
            _logger = logger;
        }

        public void recordHit()
        {
            Interlocked.Increment(ref _cacheHits);
        }

        public void recordMisses()
        {
            Interlocked.Increment(ref _cacheMisses);
        }

        public double getHitRate()
        {
            long hits = Interlocked.Read(ref _cacheHits);
            long misses = Interlocked.Read(ref _cacheMisses);

            long total = hits + misses;

            return total > 0 ? (double)hits / total : 0;
        }

        public void logMetrics()
        {
            long hits = Interlocked.Read(ref _cacheHits);
            long misses = Interlocked.Read(ref _cacheMisses);
            long total = hits + misses;

            double hitRate = total > 0 ? (double)hits / total * 100 : 0;

            _logger.LogInformation("캐시 성능 메트릭 - 히트: {Hits}, 미스: {Misses}, 히트율: {HitRate:F2}%",
                hits, misses, hitRate);
        }

        public void reset()
        {
            Interlocked.Exchange(ref _cacheHits, 0);
            Interlocked.Exchange(ref _cacheMisses, 0);
        }

        public CacheMetricsDto GetMetrics()
        {
            long hits = Interlocked.Read(ref _cacheHits);
            long misses = Interlocked.Read(ref _cacheMisses);
            long total = hits + misses;
            double hitRate = total > 0 ? (double)hits / total * 100 : 0;

            return new CacheMetricsDto
            {
                Hits = hits,
                Misses = misses,
                Total = total,
                HitRate = hitRate
            };
        }
    }

    public class CacheMetricsDto
    {
        public long Hits { get; set; }
        public long Misses { get; set; }
        public long Total { get; set; }
        public double HitRate { get; set; }
    }
}
