using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;

namespace Empen.Service
{
    public class LoginbonusService : ILoginbonusService
    {
        private readonly MasterDbContext _masterContext;
        private readonly PersonDbContext _personContext;
        private readonly IMailService _mailService;
        private readonly IRewardService _rewardService;
        private readonly ITimeService _timeService;
        private readonly ILogger<LoginbonusService> _logger;

        public LoginbonusService(MasterDbContext masterContext, PersonDbContext personContext, IMailService mailService, IRewardService rewardService, ITimeService itemService, ILogger<LoginbonusService> logger)
        {
            _masterContext = masterContext;
            _personContext = personContext;
            _mailService = mailService;
            _rewardService = rewardService;
            _timeService = itemService;
            _logger = logger;
        }

        public async Task<ICollection<LoginbonusInfoDto>> getActiveLoginbonusAsync()
        {
            DateTime now = await _timeService.getNowAsync();

            var activeBonuses = await _masterContext.master_daily_login_bonus
                .Include(b => b.daily_login_bonus_days)
                .AsNoTracking()
                .Where(b => (b.start_date <= now && b.end_date >= now) || (b.start_date <= now && b.end_date == null))
                .ToListAsync();

            if (activeBonuses.Count == 0)
            {
                return new List<LoginbonusInfoDto>();
            }

            var rewardMap = await getRewardMapFromBonuses(activeBonuses);

            return activeBonuses.Select(b => MapToDto(b, rewardMap)).ToList();
        }

        public async Task<ICollection<LoginbonusInfoDto>> getAllLoginbonusAsync()
        {
            var bonuses = await _masterContext.master_daily_login_bonus
                .Include(b => b.daily_login_bonus_days)
                .AsNoTracking()
                .ToListAsync();

            if (bonuses.Count == 0)
            {
                return new List<LoginbonusInfoDto>();
            }

            var rewardMap = await getRewardMapFromBonuses(bonuses);

            return bonuses.Select(b => MapToDto(b, rewardMap)).ToList();
        }

        public async Task<ICollection<LoginbonusInfoDto>> getAllLoginbonusByPersonIdAsync(int personId)
        {
            var now = await _timeService.getNowAsync();

            var todayStart = now.Date;
            var tomorrowStart = todayStart.AddDays(1);

            // 로그인 기록 체크
            var loginHistory = await _personContext.person_login
                .FirstOrDefaultAsync(l => l.person_id == personId && l.insert_date >= todayStart && l.insert_date < tomorrowStart);

            if (loginHistory == null)
            {
                _personContext.Add(new person_login
                {
                    person_id = personId,
                    insert_date = now,
                    update_date = now
                });
            }
            else
            {
                loginHistory.update_date = now;
            }

            var activeBonuses = await _masterContext.master_daily_login_bonus
                .Include(b => b.daily_login_bonus_days)
                .AsNoTracking()
                .Where(b => (b.start_date <= now && b.end_date >= now) || (b.start_date <= now && b.end_date == null))
                .ToListAsync();

            if (activeBonuses.Count == 0)
            {
                return new List<LoginbonusInfoDto>();
            }

            var rewardMap = await getRewardMapFromBonuses(activeBonuses);

            var loginBonusIds = activeBonuses.Select(l => l.daily_login_bonus_id).ToList();

            var userLoginHistoryMap = await _personContext.person_loginbonus
                .AsNoTracking()
                .Where(p => p.person_id == personId && loginBonusIds.Contains(p.daily_login_bonus_id))
                .GroupBy(p => p.daily_login_bonus_id)
                .Select(g => g.OrderByDescending(p => p.insert_date).First())
                .ToDictionaryAsync(p => p.daily_login_bonus_id);

            var statusMap = new Dictionary<int, int>();

            foreach (var bonus in activeBonuses)
            {
                var days = bonus.daily_login_bonus_days.OrderBy(d => d.total_login_count).ToList();
                userLoginHistoryMap.TryGetValue(bonus.daily_login_bonus_id, out var lastHistory);

                int currentDayStatus = 0;

                // 처음받음
                if (lastHistory == null)
                {
                    var firstDay = days.FirstOrDefault(d => d.total_login_count == 1);
                    if (firstDay != null)
                    {
                        _personContext.Add(new person_loginbonus
                        {
                            person_id = personId,
                            pre_login_date = null,
                            daily_login_bonus_id = bonus.daily_login_bonus_id,
                            total_login_count = 1,
                            insert_date = now,
                            update_date = now
                        });

                        _mailService.sendMailOneAmountByPersonId(personId, Constant.LOGIN_BONUS_MAIL_TITLE, string.Format(Constant.LOGIN_BONUS_MAIL_DES, "1"),
                            firstDay.reward_id_1, null, null, now, now);

                        currentDayStatus = 1;
                    }
                }
                else
                {
                    // 오늘 아직 안 받음
                    if (lastHistory.insert_date < todayStart)
                    {
                        int nextCount = lastHistory.total_login_count + 1;
                        var nextDay = days.FirstOrDefault(d => d.total_login_count == nextCount);

                        if (nextDay != null)
                        {
                            _personContext.Add(new person_loginbonus
                            {
                                person_id = personId,
                                pre_login_date = lastHistory.insert_date,
                                daily_login_bonus_id = bonus.daily_login_bonus_id,
                                total_login_count = nextCount,
                                insert_date = now,
                                update_date = now
                            });

                            _mailService.sendMailOneAmountByPersonId(personId, Constant.LOGIN_BONUS_MAIL_TITLE, string.Format(Constant.LOGIN_BONUS_MAIL_DES, nextCount.ToString()),
                                 nextDay.reward_id_1, null, null, now, now);

                            currentDayStatus = nextCount;
                        }
                        else
                        {
                            currentDayStatus = -1; // 완료
                        }
                    }
                    else
                    {
                        currentDayStatus = -1; // 오늘 이미 받음
                    }
                }
                statusMap[bonus.daily_login_bonus_id] = currentDayStatus;
            }

            try
            {
                await _personContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception from getAllLoginbonusByPersonIdAsync!");
                return new List<LoginbonusInfoDto>();
            }

            var check = activeBonuses.Select(b => MapToDto(b, rewardMap, statusMap.GetValueOrDefault(b.daily_login_bonus_id, 0))).ToList();

            return check;
        }

        private async Task<Dictionary<int, ObjectDisplayDto>> getRewardMapFromBonuses(IEnumerable<master_daily_login_bonus> bonuses)
        {
            var rewardIds = new HashSet<int>();
            foreach (var b in bonuses)
            {
                foreach (var d in b.daily_login_bonus_days)
                {
                    rewardIds.Add(d.reward_id_1);
                }
            }

            return await _rewardService.getObjectDisplayAsync(rewardIds);
        }

        private LoginbonusInfoDto MapToDto(master_daily_login_bonus b, Dictionary<int, ObjectDisplayDto> rewardMap, int currentDay = 0)
        {
            return new LoginbonusInfoDto
            {
                daily_login_bonus_id = b.daily_login_bonus_id,
                title_text = b.title_text,
                back_image = b.back_image,
                current_day = currentDay,
                bonus_days = b.daily_login_bonus_days
                    .OrderBy(d => d.total_login_count)
                    .Select(d => new LoginbonusDayDto
                    {
                        daily_login_bonus_day_id = d.daily_login_bonus_day_id,
                        total_login_count = d.total_login_count,

                        reward_1 = rewardMap.GetValueOrDefault(d.reward_id_1)
                    }).ToList()
            };
        }
    }
}