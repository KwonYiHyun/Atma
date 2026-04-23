using Empen.Data;
using Empen.Service.IService;
using Microsoft.Extensions.Caching.Memory;
using ServerCore.Service;

namespace Empen.Worker
{
    public class MasterDataSyncWorker : BackgroundService
    {
        private readonly IRedisService _redisService;
        private readonly IMemoryCache _localCache;
        private readonly ILogger<MasterDataSyncWorker> _logger;

        public MasterDataSyncWorker(IRedisService redisService, IMemoryCache localCache, ILogger<MasterDataSyncWorker> logger)
        {
            _redisService = redisService;
            _localCache = localCache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[MasterDataSync] Waiting for udpate...");

            await _redisService.subscribeAsync(CacheKey.Channel.Master, OnMessageReceived);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private void OnMessageReceived(string message)
        {
            try
            {
                switch (message.ToUpper())
                {
                    case "ALL":
                        ((MemoryCache)_localCache).Compact(1.0);
                        break;
                    case "ITEM":
                        _localCache.Remove(CacheKey.Local.ItemMap);
                        break;
                    case "REWARD":
                        _localCache.Remove(CacheKey.Local.RewardMap);
                        break;
                    case "BANNER":
                        _localCache.Remove(CacheKey.Local.BannerMap);
                        break;
                    case "CHARACTER":
                        _localCache.Remove(CacheKey.Local.CharMap);
                        break;
                    case "CHARACTER_GRADE":
                        _localCache.Remove(CacheKey.Local.CharGradeMap);
                        break;
                    case "CHARACTER_LEVEL":
                        _localCache.Remove(CacheKey.Local.CharLevelMap);
                        break;
                    case "LOGINBONUS":
                        _localCache.Remove(CacheKey.Local.LoginbonusMap);
                        break;
                    case "LOGINBONUS_DAY":
                        _localCache.Remove(CacheKey.Local.LoginbonusDayMap);
                        break;
                    case "GACHA":
                        _localCache.Remove(CacheKey.Local.GachaMap);
                        break;
                    case "GACHA_EXEC_10":
                        _localCache.Remove(CacheKey.Local.GachaExec10Map);
                        break;
                    case "GACHA_LOT":
                        _localCache.Remove(CacheKey.Local.GachaLotMap);
                        break;
                    case "GACHA_LOT_GROUP":
                        _localCache.Remove(CacheKey.Local.GachaLotGroupMap);
                        break;
                    case "PRODUCT":
                        _localCache.Remove(CacheKey.Local.ProductMap);
                        break;
                    case "PRODUCT_SET":
                        _localCache.Remove(CacheKey.Local.ProductSetMap);
                        break;
                    case "PRODUCT_SET_PIECE":
                        _localCache.Remove(CacheKey.Local.ProductSetPieceMap);
                        break;
                    case "PRODUCT_SET_PRISM":
                        _localCache.Remove(CacheKey.Local.ProductSetPrismMap);
                        break;
                    case "PRODUCT_SET_TOKEN":
                        _localCache.Remove(CacheKey.Local.ProductSetTokenMap);
                        break;
                    case "ACHIEVEMENT":
                        _localCache.Remove(CacheKey.Local.AchievementMap);
                        break;
                    case "ACHIEVEMENT_CATEGORY":
                        _localCache.Remove(CacheKey.Local.AchievementCategoryMap);
                        break;
                    default:
                        _logger.LogWarning($"Unkown update target: {message}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sub Cache Error");
            }
        }
    }
}