using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServerCore.Service;
using SharedData.Models;

namespace Empen.Service
{
    public class MasterDataCacheService : IMasterDataCacheService
    {
        private readonly IDbContextFactory<MasterDbContext> _masterFactory;
        private readonly IRedisService _redisService;
        private readonly IMemoryCache _memoryCache;
        private readonly IRedisLockService _redisLockService;

        // 기본 만료 시간 설정
        private readonly TimeSpan _defaultLocalExpiry = TimeSpan.FromDays(10);
        private readonly TimeSpan _defaultRedisExpiry = TimeSpan.FromDays(10);

        public MasterDataCacheService(IDbContextFactory<MasterDbContext> masterFactory, IRedisService redisService, IMemoryCache memoryCache, IRedisLockService redisLockService)
        {
            _masterFactory = masterFactory;
            _redisService = redisService;
            _memoryCache = memoryCache;
            _redisLockService = redisLockService;
        }

        public async Task<Dictionary<int, master_item>> getItemMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.ItemMap, CacheKey.Master.ItemAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_item.AsNoTracking().ToListAsync();
                },
                x => x.item_id
            );
        }

        public async Task<Dictionary<int, master_reward>> getRewardMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.RewardMap, CacheKey.Master.RewardAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_reward.AsNoTracking().ToListAsync();
                },
                x => x.reward_id
            );
        }

        public async Task<Dictionary<int, master_banner>> getBannerMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.BannerMap, CacheKey.Master.BannerAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_banner.AsNoTracking().ToListAsync();
                },
                x => x.banner_id
            );
        }

        public async Task<Dictionary<int, master_character>> getCharacterMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.CharMap, CacheKey.Master.CharacterAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_character.AsNoTracking().ToListAsync();
                },
                x => x.character_id
            );
        }

        public async Task<Dictionary<(int, int), master_character_grade>> getCharacterGradeMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.CharGradeMap, CacheKey.Master.CharacterGradeAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_character_grade.AsNoTracking().ToListAsync();
                },
                x => (x.character_id, x.grade)
            );
        }

        public async Task<Dictionary<(int, int), master_character_level>> getCharacterLevelMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.CharLevelMap, CacheKey.Master.CharacterLevelAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_character_level.AsNoTracking().ToListAsync();
                },
                x => (x.character_id, x.level)
            );
        }

        public async Task<Dictionary<int, master_daily_login_bonus>> getLoginbonusMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.LoginbonusMap, CacheKey.Master.LoginbonusAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_daily_login_bonus.AsNoTracking().ToListAsync();
                },
                x => x.daily_login_bonus_id
            );
        }

        public async Task<Dictionary<int, master_daily_login_bonus_day>> getLoginbonusDayMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.LoginbonusDayMap, CacheKey.Master.LoginbonusDayAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_daily_login_bonus_day.AsNoTracking().ToListAsync();
                },
                x => x.daily_login_bonus_day_id
            );
        }

        public async Task<Dictionary<int, master_gacha>> getGachaMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.GachaMap, CacheKey.Master.GachaAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_gacha.AsNoTracking().ToListAsync();
                },
                x => x.gacha_id
            );
        }

        public async Task<Dictionary<int, master_gacha_exec_10>> getGachaExec10MapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.GachaExec10Map, CacheKey.Master.GachaExec10All,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_gacha_exec_10.AsNoTracking().ToListAsync();
                },
                x => x.gacha_exec_10_id
            );
        }

        public async Task<Dictionary<int, master_gacha_lot>> getGachaLotMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.GachaLotMap, CacheKey.Master.GachaLotAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_gacha_lot.AsNoTracking().ToListAsync();
                },
                x => x.gacha_lot_id
            );
        }

        public async Task<Dictionary<int, master_gacha_lot_group>> getGachaLotGroupMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.GachaLotGroupMap, CacheKey.Master.GachaLotGroupAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_gacha_lot_group.AsNoTracking().ToListAsync();
                },
                x => x.gacha_lot_group_id
            );
        }

        public async Task<Dictionary<int, master_product>> getProductMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.ProductMap, CacheKey.Master.ProductAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_product.AsNoTracking().ToListAsync();
                },
                x => x.product_id
            );
        }

        public async Task<Dictionary<int, master_product_set>> getProductSetMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.ProductSetMap, CacheKey.Master.ProductSetAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_product_set.AsNoTracking().ToListAsync();
                },
                x => x.product_set_id
            );
        }

        public async Task<Dictionary<int, master_product_set_piece>> getProductSetPieceMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.ProductSetPieceMap, CacheKey.Master.ProductSetPieceAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_product_set_piece.AsNoTracking().ToListAsync();
                },
                x => x.product_set_piece_id
            );
        }

        public async Task<Dictionary<int, master_product_set_prism>> getProductSetPrismMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.ProductSetPrismMap, CacheKey.Master.ProductSetPrismAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_product_set_prism.AsNoTracking().ToListAsync();
                },
                x => x.product_set_prism_id
            );
        }

        public async Task<Dictionary<int, master_product_set_token>> getProductSetTokenMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.ProductSetTokenMap, CacheKey.Master.ProductSetTokenAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_product_set_token.AsNoTracking().ToListAsync();
                },
                x => x.product_set_token_id
            );
        }

        public async Task<Dictionary<int, master_achievement>> getAchievementMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.AchievementMap, CacheKey.Master.AchievementAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_achievement.AsNoTracking().ToListAsync();
                },
                x => x.achievement_id
            );
        }

        public async Task<Dictionary<int, master_achievement_category>> getAchievementCategoryMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.AchievementCategoryMap, CacheKey.Master.AchievementCategoryAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_achievement_category.AsNoTracking().ToListAsync();
                },
                x => x.achievement_category_id
            );
        }

        public async Task<Dictionary<int, master_notice>> getNoticeMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.NoticeMap, CacheKey.Master.NoticeAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_notice.AsNoTracking().ToListAsync();
                },
                x => x.notice_id
            );
        }

        public async Task<Dictionary<int, master_mail>> getMailMapAsync()
        {
            return await getOrSetMapAsync(CacheKey.Local.MailMap, CacheKey.Master.MailAll,
                async () =>
                {
                    using var context = _masterFactory.CreateDbContext();
                    return await context.master_mail.AsNoTracking().ToListAsync();
                },
                x => x.mail_id
            );
        }

        /// <summary>
        /// 3단 캐싱 (Local -> Redis -> DB) 후 Dictionary로 반환
        /// </summary>
        /// <typeparam name="TKey">Dictionary의 Key 타입 (예: int item_id)</typeparam>
        /// <typeparam name="TValue">데이터 모델 타입 (예: master_item)</typeparam>
        private async Task<Dictionary<TKey, TValue>> getOrSetMapAsync<TKey, TValue>(
            string localKey,
            string redisKey,
            Func<Task<List<TValue>>> dbFetcher, // DB에서 데이터를 가져오는 콜백
            Func<TValue, TKey> keySelector, // 리스트에서 Key를 추출하는 함수
            TimeSpan? localExpiry = null, // 로컬 만료 시간
            TimeSpan? redisExpiry = null // Redis 만료 시간
            ) where TKey : notnull
        {
            // Local
            return await _memoryCache.GetOrCreateAsync(localKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = localExpiry ?? _defaultLocalExpiry;

                // Redis Cache
                var list = await _redisService.getAsync<List<TValue>>(redisKey);

                if (list == null)
                {
                    // 캐시 스탬피드 방지
                    string lockKey = "Lock:" + redisKey;
                    string? token = await _redisLockService.lockAsync(lockKey);

                    if (token != null)
                    {
                        try
                        {
                            // Lock 획득 사이에 다른 서버가 캐싱을 했을 수도 있음
                            list = await _redisService.getAsync<List<TValue>>(redisKey);

                            if (list == null)
                            {
                                // DB
                                list = await dbFetcher();

                                if (list != null && list.Count > 0)
                                {
                                    await _redisService.setAsync(redisKey, list, redisExpiry ?? _defaultRedisExpiry);
                                }
                                else
                                {
                                    list = new List<TValue>();
                                }
                            }
                        }
                        finally
                        {
                            await _redisLockService.unLockAsync(lockKey, token);
                        }
                    }
                    else
                    {
                        await Task.Delay(300);

                        list = await _redisService.getAsync<List<TValue>>(redisKey);

                        if (list == null)
                        {
                            list = new List<TValue>();
                        }
                    }
                }

                return list.ToDictionary(keySelector);
            });
        }
    }
}
