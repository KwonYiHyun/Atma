
using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;

namespace Empen.Service
{
    public class CachePrewarmingService : IHostedService
    {
        private readonly IMasterDataCacheService _masterCacheService;
        private readonly IRedisLockService _redisLockService;
        private readonly ILogger<CachePrewarmingService> _logger;

        public CachePrewarmingService(IMasterDataCacheService masterCacheService, IRedisLockService redisLockService, ILogger<CachePrewarmingService> logger)
        {
            _masterCacheService = masterCacheService;
            _redisLockService = redisLockService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CachePrewarming] Starting...");

            string? token = await _redisLockService.lockAsync("System:Prewarming");

            if (token != null)
            {
                try
                {
                    _logger.LogInformation("[CachePrewarming] This server performs master data loading.");

                    await LoadAllMasterDataAsync();

                    _logger.LogInformation("[CachePrewarming] All data loaded.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[CachePrewarming] Error loading data.");
                }
                finally
                {
                    await _redisLockService.unLockAsync("System:Prewarming", token);
                }
            }
            else
            {
                _logger.LogInformation("[CachePrewarming] Wait because another server is already loading.");
                await Task.Delay(5000, cancellationToken);

                _logger.LogInformation("[Local] Load data Redis to local.");

                try
                {
                    await LoadAllMasterDataAsync();

                    _logger.LogInformation("[Local] Local cache ready.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Local Prewarming Failed.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task LoadAllMasterDataAsync()
        {
            var tasks = new List<Task>()
            {
                _masterCacheService.getItemMapAsync(),
                _masterCacheService.getRewardMapAsync(),
                _masterCacheService.getBannerMapAsync(),
                _masterCacheService.getCharacterMapAsync(),
                _masterCacheService.getCharacterGradeMapAsync(),
                _masterCacheService.getCharacterLevelMapAsync(),
                _masterCacheService.getLoginbonusMapAsync(),
                _masterCacheService.getLoginbonusDayMapAsync(),
                _masterCacheService.getGachaMapAsync(),
                _masterCacheService.getGachaExec10MapAsync(),
                _masterCacheService.getGachaLotMapAsync(),
                _masterCacheService.getGachaLotGroupMapAsync(),
                _masterCacheService.getProductMapAsync(),
                _masterCacheService.getProductSetMapAsync(),
                _masterCacheService.getProductSetPieceMapAsync(),
                _masterCacheService.getProductSetPrismMapAsync(),
                _masterCacheService.getProductSetTokenMapAsync(),
                _masterCacheService.getAchievementMapAsync(),
                _masterCacheService.getAchievementCategoryMapAsync(),
                _masterCacheService.getNoticeMapAsync(),
                _masterCacheService.getMailMapAsync()
            };

            await Task.WhenAll(tasks);
        }
    }
}
