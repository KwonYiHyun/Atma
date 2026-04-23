using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;

namespace Empen.Service
{
    public class AchievementService : IAchievementService
    {
        private readonly PersonDbContext _personContext;
        private readonly IMailService _mailService;
        private readonly ITimeService _timeService;
        private readonly IRewardService _rewardService;
        private readonly IRedisService _redisService;
        private readonly ILogger<IAchievementService> _logger;
        private readonly IMasterDataCacheService _masterCacheService;

        private const string REDIS_KEY_PENDING_NOTIFICATIONS = "Achievement:Pending:{0}";
        private const string REDIS_KEY_ALREADY_NOTIFIED = "Achievement:Notified:{0}:{1}";

        public AchievementService(PersonDbContext personContext, IMailService mailService, ITimeService timeService, IRewardService rewardService, IRedisService redisService, ILogger<IAchievementService> logger, IMasterDataCacheService masterCacheService)
        {
            _personContext = personContext;
            _mailService = mailService;
            _timeService = timeService;
            _rewardService = rewardService;
            _redisService = redisService;
            _logger = logger;
            _masterCacheService = masterCacheService;
        }

        private class UserAchievementStats
        {
            public int DailyLogin { get; set; }
            public int WeeklyLogin { get; set; }
            public int TotalLogin { get; set; }

            public int DailyGacha { get; set; }
            public int WeeklyGacha { get; set; }
            public int TotalGacha { get; set; }

            public HashSet<int> ClearedStories { get; set; } = new HashSet<int>();
        }

        private async Task<(DateTime dailyStart, DateTime weeklyStart)> getTimeBoundariesAsync()
        {
            DateTime now = await _timeService.getNowAsync();
            DateTime dailyStart = now.Date;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weeklyStart = now.Date.AddDays(-1 * diff);
            return (dailyStart, weeklyStart);
        }

        private async Task<UserAchievementStats> loadUserStatsAsync(int personId, DateTime dailyStart, DateTime weeklyStart)
        {
            var stats = new UserAchievementStats();

            // 로그인
            var loginQuery = _personContext.person_login.AsNoTracking().Where(p => p.person_id == personId);
            stats.TotalLogin = await loginQuery.CountAsync();
            stats.WeeklyLogin = await loginQuery.CountAsync(l => l.insert_date >= weeklyStart);
            stats.DailyLogin = await loginQuery.CountAsync(l => l.insert_date >= dailyStart);

            // 가챠
            var gachaQuery = _personContext.person_gacha.AsNoTracking().Where(p => p.person_id == personId);
            stats.TotalGacha = await gachaQuery.SumAsync(g => g.gacha_count);
            stats.WeeklyGacha = await gachaQuery.Where(p => p.insert_date >= weeklyStart).SumAsync(p => p.gacha_count);
            stats.DailyGacha = await gachaQuery.Where(p => p.insert_date >= dailyStart).SumAsync(p => p.gacha_count);

            // 스토리
            //stats.ClearedStories = await _personContext.person_story_history
            //    .AsNoTracking()
            //    .Where(p => p.person_id == personId)
            //    .Select(p => p.story_id)
            //    .ToHashSetAsync();

            return stats;
        }

        private int calculateCurrentValue(master_achievement master, UserAchievementStats stats)
        {
            int currentVal = 0;
            var category = (AchievementCategory)master.achievement_category_id;

            switch ((AchievementType)master.achievement_type)
            {
                case AchievementType.LOGIN_COUNT:
                    currentVal = category switch
                    {
                        AchievementCategory.DAILY => stats.DailyLogin,
                        AchievementCategory.WEEKLY => stats.WeeklyLogin,
                        _ => stats.TotalLogin
                    };
                    break;

                case AchievementType.GACHA_PLAY:
                    currentVal = category switch
                    {
                        AchievementCategory.DAILY => stats.DailyGacha,
                        AchievementCategory.WEEKLY => stats.WeeklyGacha,
                        _ => stats.TotalGacha
                    };
                    break;

                case AchievementType.STORY_CLEAR:
                    currentVal = stats.ClearedStories.Contains(master.achievement_param_1) ? 1 : 0;
                    break;
            }

            return currentVal;
        }

        private int getTargetValue(master_achievement master)
        {
            return (AchievementType)master.achievement_type switch
            {
                // 스토리 시청 여부
                AchievementType.STORY_CLEAR => 1,

                // 나머지
                _ => master.achievement_param_1
            };
        }

        private bool isRewardTaken(master_achievement master, Dictionary<int, DateTime> takenMap, DateTime dailyStart, DateTime weeklyStart)
        {
            if (!takenMap.TryGetValue(master.achievement_id, out DateTime lastTaken))
            {
                return false; // 기록 없음 -> 안 받음
            }

            return (AchievementCategory)master.achievement_category_id switch
            {
                AchievementCategory.DAILY => lastTaken >= dailyStart,
                AchievementCategory.WEEKLY => lastTaken >= weeklyStart,
                _ => true // SPECIAL 등은 한 번 받으면 끝
            };
        }

        private List<int> getRewardIds(master_achievement master)
        {
            var list = new List<int> { master.reward_id_1 };
            if (master.reward_id_2.HasValue)
            {
                list.Add(master.reward_id_2.Value);
            }
            if (master.reward_id_3.HasValue)
            {
                list.Add(master.reward_id_3.Value);
            }
            return list;
        }

        public async Task checkAndSetNewFlagAsync(int personId, AchievementType? targetType = null)
        {
            DateTime now = await _timeService.getNowAsync();
            var (dailyStart, weeklyStart) = await getTimeBoundariesAsync();
            var stats = await loadUserStatsAsync(personId, dailyStart, weeklyStart);

            var achievementMap = await _masterCacheService.getAchievementMapAsync();

            var masterList = achievementMap.Values
                .Where(m => m.start_date <= now && (m.end_date == null || m.end_date >= now));

            if (targetType.HasValue)
            {
                int typeValue = (int)targetType.Value;
                masterList = masterList.Where(m => m.achievement_type == typeValue);
            }

            var finalList = masterList.ToList();
            if (finalList.Count == 0)
            {
                return;
            }

            var takenMap = await _personContext.person_achievement.AsNoTracking()
                .Where(p => p.person_id == personId)
                .GroupBy(p => p.achievement_id)
                .Select(g => new { Id = g.Key, LastTakenDate = g.Max(x => x.insert_date) })
                .ToDictionaryAsync(x => x.Id, x => x.LastTakenDate);

            string pendingKey = string.Format(REDIS_KEY_PENDING_NOTIFICATIONS, personId);

            foreach (var master in finalList)
            {
                if (isRewardTaken(master, takenMap, dailyStart, weeklyStart))
                {
                    continue;
                }

                int targetVal = getTargetValue(master);
                int currentVal = calculateCurrentValue(master, stats);

                if (currentVal >= targetVal)
                {
                    string notifiedSuffix;
                    TimeSpan ttl;

                    if (master.achievement_category_id == (int)AchievementCategory.DAILY)
                    {
                        notifiedSuffix = $"D:{dailyStart:yyyyMMdd}";
                        ttl = TimeSpan.FromDays(2);
                    }
                    else if (master.achievement_category_id == (int)AchievementCategory.WEEKLY)
                    {
                        notifiedSuffix = $"W:{weeklyStart:yyyyMMdd}";
                        ttl = TimeSpan.FromDays(8);
                    }
                    else
                    {
                        notifiedSuffix = "G";
                        ttl = TimeSpan.FromDays(30);
                    }

                    string notifiedKey = string.Format(REDIS_KEY_ALREADY_NOTIFIED, personId, notifiedSuffix) + $":{master.achievement_id}";
                    bool isNewKey = await _redisService.setStringNxAsync(notifiedKey, "1", ttl);

                    if (!isNewKey)
                    {
                        continue;
                    }

                    AchievementSuccessInfo info = new AchievementSuccessInfo
                    {
                        title = master.achievement_title,
                        targetValue = targetVal
                    };

                    await _redisService.listRightPushAsync<AchievementSuccessInfo>(pendingKey, info);
                    await _redisService.setStringAsync(notifiedKey, "1", ttl);
                }
            }
        }

        public async Task<ICollection<AchievementDto>> getAllAchievementAsync(int personId)
        {
            DateTime now = await _timeService.getNowAsync();
            var (dailyStart, weeklyStart) = await getTimeBoundariesAsync();

            DateTime dailyEnd = dailyStart.AddDays(1);
            DateTime weeklyEnd = weeklyStart.AddDays(7);

            var achievementMap = await _masterCacheService.getAchievementMapAsync();
            var masterList = achievementMap.Values
                .Where(m => m.start_date <= now && (m.end_date == null || m.end_date >= now))
                .OrderBy(m => m.show_order)
                .ToList();

            if (masterList.Count == 0)
            {
                return new List<AchievementDto>();
            }

            // 보상을 받은 내역
            var takenMap = await _personContext.person_achievement
                .AsNoTracking()
                .Where(p => p.person_id == personId)
                .GroupBy(p => p.achievement_id)
                .Select(g => new { Id = g.Key, LastTakenDate = g.Max(x => x.insert_date) })
                .ToDictionaryAsync(x => x.Id, x => x.LastTakenDate);

            var stats = await loadUserStatsAsync(personId, dailyStart, weeklyStart);

            var allRewardIds = masterList.SelectMany(getRewardIds).Distinct().ToList();
            var rewardMap = await _rewardService.getObjectDisplayAsync(allRewardIds);

            List<AchievementDto> result = new List<AchievementDto>();

            foreach (var master in masterList)
            {
                int currentVal = calculateCurrentValue(master, stats);
                int targetVal = master.achievement_param_1;

                // Max 보정
                if (currentVal > targetVal)
                {
                    currentVal = targetVal;
                }

                // 남은 시간
                long remainSec = -1;

                if (master.achievement_category_id == (int)AchievementCategory.DAILY)
                {
                    remainSec = (long)(dailyEnd - now).TotalSeconds;
                }
                else if (master.achievement_category_id == (int)AchievementCategory.WEEKLY)
                {
                    remainSec = (long)(weeklyEnd - now).TotalSeconds;
                }

                if (remainSec < -1)
                {
                    remainSec = 0;
                }

                result.Add(new AchievementDto
                {
                    achievement_id = master.achievement_id,
                    achievement_category_id = (AchievementCategory)master.achievement_category_id,
                    achievement_title = master.achievement_title,
                    achievement_description = master.achievement_description,
                    achievement_type = (AchievementType)master.achievement_type,
                    current_value = currentVal,
                    target_value = targetVal,
                    is_clear = currentVal >= targetVal,
                    is_taken = isRewardTaken(master, takenMap, dailyStart, weeklyStart),

                    reward_1 = rewardMap.GetValueOrDefault(master.reward_id_1),
                    reward_2 = master.reward_id_2.HasValue ? rewardMap.GetValueOrDefault(master.reward_id_2.Value) : null,
                    reward_3 = master.reward_id_3.HasValue ? rewardMap.GetValueOrDefault(master.reward_id_3.Value) : null,

                    remaining_seconds = remainSec
                });
            }

            return result;
        }

        public async Task<(ErrorCode, ICollection<ObjectDisplayDto>)> claimAllAchievementAsync(int personId)
        {
            DateTime now = await _timeService.getNowAsync();
            var (dailyStart, weeklyStart) = await getTimeBoundariesAsync();

            var achievementMap = await _masterCacheService.getAchievementMapAsync();

            var masterList = achievementMap.Values
                .Where(m => m.start_date <= now && (m.end_date == null || m.end_date >= now))
                .ToList();

            if (masterList.Count == 0)
            {
                return (ErrorCode.AchievementNotFound, null);
            }

            var takenMap = await _personContext.person_achievement
                .AsNoTracking()
                .Where(p => p.person_id == personId)
                .GroupBy(p => p.achievement_id)
                .Select(g => new { Id = g.Key, LastTakenDate = g.Max(x => x.insert_date) })
                .ToDictionaryAsync(x => x.Id, x => x.LastTakenDate);

            var stats = await loadUserStatsAsync(personId, dailyStart, weeklyStart);

            List<master_achievement> claimableList = new List<master_achievement>();
            List<int> claimRewardIds = new List<int>();

            foreach (var master in masterList)
            {
                if (isRewardTaken(master, takenMap, dailyStart, weeklyStart))
                {
                    continue;
                }

                int currentVal = calculateCurrentValue(master, stats);
                if (currentVal >= master.achievement_param_1)
                {
                    claimableList.Add(master);
                    claimRewardIds.AddRange(getRewardIds(master));
                }
            }

            if (claimableList.Count == 0)
            {
                return (ErrorCode.AchievementNotFound, null);
            }

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var newHistories = new List<person_achievement>();

                    foreach (var achievement in claimableList)
                    {
                        _mailService.sendMailOneAmountByPersonId(personId, string.Format(Constant.ACHIEVEMENT_TITLE, achievement.achievement_title),
                            Constant.ACHIEVEMENT_DES,
                            achievement.reward_id_1, achievement.reward_id_2, achievement.reward_id_3, now, now);

                        newHistories.Add(new person_achievement
                        {
                            person_id = personId,
                            achievement_id = achievement.achievement_id,
                            insert_date = now,
                            update_date = now
                        });
                    }

                    _personContext.person_achievement.AddRange(newHistories);
                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "ClaimAllRewardAsync Error PersonId {personId}", personId);
                    return (ErrorCode.TransactionFailed, null);
                }
            }

            List<ObjectDisplayDto> rewardList = new List<ObjectDisplayDto>();

            foreach (var id in claimRewardIds)
            {
                var rewardObjects = await _rewardService.getObjectDisplayAsync(new List<int>() { id });
                rewardList.AddRange(rewardObjects.Values.ToList());
            }

            return (ErrorCode.Success, rewardList);
        }

        public async Task<(ErrorCode, ICollection<ObjectDisplayDto>)> claimAchievementAsync(int personId, int achievementId)
        {
            DateTime now = await _timeService.getNowAsync();
            var (dailyStart, weeklyStart) = await getTimeBoundariesAsync();

            var achievementMap = await _masterCacheService.getAchievementMapAsync();
            if (!achievementMap.TryGetValue(achievementId, out var master))
            {
                return (ErrorCode.AchievementNotFound, null);
            }

            var lastHistoryDate = await _personContext.person_achievement
                .AsNoTracking()
                .Where(p => p.person_id == personId && p.achievement_id == achievementId)
                .OrderByDescending(p => p.insert_date)
                .Select(p => p.insert_date)
                .FirstOrDefaultAsync();

            if (lastHistoryDate != default(DateTime))
            {
                var tempMap = new Dictionary<int, DateTime> { { achievementId, lastHistoryDate } };
                if (isRewardTaken(master, tempMap, dailyStart, weeklyStart))
                {
                    return (ErrorCode.AlreadyReceived, null);
                }
            }

            var stats = await loadUserStatsAsync(personId, dailyStart, weeklyStart);
            int currentVal = calculateCurrentValue(master, stats);

            if (currentVal < master.achievement_param_1)
            {
                return (ErrorCode.NotConditionMet, null);
            }

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    _mailService.sendMailOneAmountByPersonId(personId, string.Format(Constant.ACHIEVEMENT_TITLE, master.achievement_title),
                        string.Format(Constant.ACHIEVEMENT_DES, master.achievement_description),
                        master.reward_id_1, master.reward_id_2, master.reward_id_3, now, now);

                    _personContext.person_achievement.Add(new person_achievement
                    {
                        person_id = personId,
                        achievement_id = achievementId,
                        insert_date = now,
                        update_date = now
                    });

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "ClaimRewardAsync Error PersonId {personId}", personId);
                    return (ErrorCode.TransactionFailed, null);
                }
            }

            var rewardIds = getRewardIds(master);
            var rewards = await _rewardService.getObjectDisplayAsync(rewardIds);

            return (ErrorCode.Success, rewards.Values.ToList());
        }
    }
}