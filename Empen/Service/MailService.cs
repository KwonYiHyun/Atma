using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Empen.Service
{
    public class MailService : IMailService
    {
        private readonly PersonDbContext _personContext;
        private readonly IItemService _itemService;
        private readonly IResourceService _resourceService;
        private readonly ICharacterService _characterService;
        private readonly IRewardService _rewardService;
        private readonly ITimeService _timeService;
        private readonly ILogger<IMailService> _logger;
        private readonly IMasterDataCacheService _masterCacheService;
        private readonly IPersonDataCacheService _personDataCacheService;

        public MailService(PersonDbContext personContext, IItemService itemService, IResourceService resourceService, ICharacterService characterService, IRewardService rewardService, ITimeService timeService, ILogger<IMailService> logger, IMasterDataCacheService masterCacheService, IPersonDataCacheService personDataCacheService)
        {
            _personContext = personContext;
            _itemService = itemService;
            _resourceService = resourceService;
            _characterService = characterService;
            _rewardService = rewardService;
            _timeService = timeService;
            _logger = logger;
            _masterCacheService = masterCacheService;
            _personDataCacheService = personDataCacheService;
        }

        public void sendMailOneAmountByPersonId(int personId, string title, string description, int reward_id_1, int? reward_id_2, int? reward_id_3, DateTime insertDate, DateTime updateDate)
        {
            sendMailByPersonId(personId, title, description,
                reward_id_1, 1,
                reward_id_2, (reward_id_2 != null) ? 1 : null,
                reward_id_3, (reward_id_3 != null) ? 1 : null,
                insertDate, updateDate);
        }

        public void sendMailByPersonId(int personId, string title, string description, int reward_id_1, int reward_id_1_amount, int? reward_id_2, int? reward_id_2_amount, int? reward_id_3, int? reward_id_3_amount, DateTime insertDate, DateTime updateDate)
        {
            person_mail mail = new person_mail()
            {
                person_id = personId,
                title = title,
                description = description,
                reward_id_1 = reward_id_1,
                reward_id_1_amount = reward_id_1_amount,
                reward_id_2 = reward_id_2,
                reward_id_2_amount = (reward_id_2 != null) ? reward_id_2_amount : null,
                reward_id_3 = reward_id_3,
                reward_id_3_amount = (reward_id_3 != null) ? reward_id_3_amount : null,
                expire_date = insertDate.AddDays(30),
                insert_date = insertDate,
                update_date = updateDate
            };

            _personContext.Add(mail);
        }

        public async Task sendMailAllPersonId(string title, string description, int reward_id_1, int reward_id_1_amount, int? reward_id_2, int? reward_id_2_amount, int? reward_id_3, int? reward_id_3_amount, DateTime insertDate, DateTime updateDate)
        {
            var expireDate = insertDate.AddDays(30);

            string sql = @"
        INSERT INTO person_mail 
        (
            person_id, 
            title, 
            description, 
            reward_id_1, 
            reward_id_1_amount, 
            reward_id_2, 
            reward_id_2_amount, 
            reward_id_3, 
            reward_id_3_amount, 
            is_receive, 
            expire_date, 
            insert_date, 
            update_date
        )
        SELECT 
            p.person_id,
            {0},              -- @title
            {1},              -- @description
            {2},              -- @reward_id_1
            {3},              -- @reward_id_1_amount
            {4},              -- @reward_id_2 (Nullable)
            {5},              -- @reward_id_2_amount (Nullable)
            {6},              -- @reward_id_3 (Nullable)
            {7},              -- @reward_id_3_amount (Nullable)
            0,                -- is_receive (Default: false)
            {8},              -- @expire_date
            {9},              -- @insert_date
            {10}              -- @update_date
        FROM person AS p";

            object paramReward2 = (object?)reward_id_2 ?? DBNull.Value;
            object paramReward2Amount = (reward_id_2 != null) ? (object)reward_id_2_amount : DBNull.Value;

            object paramReward3 = (object?)reward_id_3 ?? DBNull.Value;
            object paramReward3Amount = (reward_id_3 != null) ? (object)reward_id_3_amount : DBNull.Value;

            await _personContext.Database.ExecuteSqlRawAsync(sql,
                title,
                description,
                reward_id_1,
                reward_id_1_amount,
                paramReward2,
                paramReward2Amount,
                paramReward3,
                paramReward3Amount,
                expireDate,
                insertDate,
                updateDate
            );
        }

        public async Task<ICollection<MailInfoDto>> getAllMail(int personId)
        {
            var now = await _timeService.getNowAsync();

            var myMails = await _personContext.person_mail
                .AsNoTracking()
                .Where(m => m.person_id == personId && (m.expire_date > now || m.expire_date == null))
                .OrderByDescending(m => m.insert_date)
                .ToListAsync();

            if (myMails.Count == 0)
            {
                return new List<MailInfoDto>();
            }

            // mail reward_id 수집
            var rewardIds = new HashSet<int>();
            foreach (var mail in myMails)
            {
                rewardIds.Add(mail.reward_id_1);
                if (mail.reward_id_2.HasValue)
                {
                    rewardIds.Add(mail.reward_id_2.Value);
                }
                if (mail.reward_id_3.HasValue)
                {
                    rewardIds.Add(mail.reward_id_3.Value);
                }
            }

            var rewardMap = await _rewardService.getObjectDisplayAsync(rewardIds);

            List<MailInfoDto> resultList = new List<MailInfoDto>();

            foreach (var mail in myMails)
            {
                var mailDto = new MailInfoDto
                {
                    person_mail_id = mail.person_mail_id,
                    title = mail.title,
                    description = mail.description,
                    is_receive = mail.is_receive,
                    insert_date = mail.insert_date,
                    update_date = mail.update_date,
                    current_date = now,
                    expire_date = mail.expire_date,
                    reward = new ObjectDisplayDto()
                };

                void AddRewardIfExist(int? rewardId, int? amount)
                {
                    if (rewardId.HasValue && rewardMap.TryGetValue(rewardId.Value, out var displayDto))
                    {
                        var dto = new ObjectDisplayDto
                        {
                            object_type = displayDto.object_type
                        };

                        if (displayDto.common_item_info != null)
                        {
                            var info = displayDto.common_item_info;
                            var finalAmount = info.amount * (amount ?? 1);

                            dto.common_item_info = new CommonItemInfoDto
                            {
                                id = info.id,
                                name = info.name,
                                description = info.description,
                                image = info.image,
                                amount = finalAmount,

                                item_type = info.item_type,
                                tab_type = info.tab_type,
                                item_param_1 = info.item_param_1,
                                item_param_2 = info.item_param_2,
                                expire_date = info.expire_date,
                                usable = info.usable
                            };
                        }
                        else if (displayDto.character_info != null)
                        {
                            dto.character_info = displayDto.character_info;
                        }

                        mailDto.reward = dto;
                    }
                }

                AddRewardIfExist(mail.reward_id_1, mail.reward_id_1_amount);
                AddRewardIfExist(mail.reward_id_2, mail.reward_id_2_amount);
                AddRewardIfExist(mail.reward_id_3, mail.reward_id_3_amount);

                resultList.Add(mailDto);
            }

            return resultList;
        }

        public async Task<ICollection<ObjectDisplayDto>> takeMailById(int personId, int personMailId)
        {
            var now = await _timeService.getNowAsync();

            var mail = await _personContext.person_mail
                .Where(m => m.person_mail_id == personMailId && m.person_id == personId && (m.expire_date > now || m.expire_date == null) && m.is_receive == false)
                .FirstOrDefaultAsync();

            if (mail == null)
            {
                return new List<ObjectDisplayDto>();
            }

            return await ReceiveAllMailsDisplay(personId, new List<person_mail> { mail });
        }
        
        public async Task<ICollection<ObjectDisplayDto>> takeAllMailDisplay(int personId)
        {
            var now = await _timeService.getNowAsync();

            var mails = await _personContext.person_mail
                .Where(m => m.person_id == personId && (m.expire_date > now || m.expire_date == null) && m.is_receive == false)
                .ToListAsync();

            if (mails.Count == 0)
            {
                return new List<ObjectDisplayDto>();
            }

            return await ReceiveAllMailsDisplay(personId, mails);
        }

        public async Task<ICollection<ObjectDisplayDto>> ReceiveAllMailsDisplay(int personId, List<person_mail> mails)
        {
            var now = await _timeService.getNowAsync();
            var emptyResult = new List<ObjectDisplayDto>();

            var rewardIds = new HashSet<int>();
            var validMails = new List<person_mail>();

            foreach (var m in mails)
            {
                if (m.is_receive)
                {
                    continue;
                }

                validMails.Add(m);
                rewardIds.Add(m.reward_id_1);
                if (m.reward_id_2.HasValue)
                {
                    rewardIds.Add(m.reward_id_2.Value);
                }
                if (m.reward_id_3.HasValue)
                {
                    rewardIds.Add(m.reward_id_3.Value);
                }
            }

            if (validMails.Count == 0)
            {
                return emptyResult;
            }

            // Reward 조회
            //var rewardMap = await _masterContext.master_reward
            //    .AsNoTracking()
            //    .Where(r => rewardIds.Contains(r.reward_id))
            //    .ToDictionaryAsync(r => r.reward_id);

            var rewardMap = await _masterCacheService.getRewardMapAsync();

            // 보상 집계 (캐릭터 제외)
            var itemAmounts = new Dictionary<int, int>();
            int totalToken = 0;
            int totalPrism = 0;
            int totalGachaResource = 0;

            // 트랜잭션 안에서 처리
            var characterRewardList = new List<int>();

            foreach (var mail in validMails)
            {
                void PreCalcReward(int rId, int rAmount)
                {
                    if (!rewardMap.TryGetValue(rId, out var masterReward)) return;

                    int finalAmount = rAmount * masterReward.object_amount;

                    switch (masterReward.object_type)
                    {
                        case (int)ObjectType.ITEM:
                            if (itemAmounts.ContainsKey(masterReward.object_value))
                            {
                                itemAmounts[masterReward.object_value] += finalAmount;
                            }
                            else
                            {
                                itemAmounts[masterReward.object_value] = finalAmount;
                            }
                            break;

                        case (int)ObjectType.CHARACTER:
                            for (int i = 0; i < finalAmount; i++)
                            {
                                characterRewardList.Add(masterReward.object_value);
                            }
                            break;
                        case (int)ObjectType.PRISM:
                            totalPrism += finalAmount; break;
                        case (int)ObjectType.TOKEN:
                            totalToken += finalAmount; break;
                        case (int)ObjectType.FILM:
                            totalGachaResource += finalAmount; break;
                    }
                }

                PreCalcReward(mail.reward_id_1, mail.reward_id_1_amount);
                if (mail.reward_id_2.HasValue)
                {
                    PreCalcReward(mail.reward_id_2.Value, mail.reward_id_2_amount.Value);
                }
                if (mail.reward_id_3.HasValue)
                {
                    PreCalcReward(mail.reward_id_3.Value, mail.reward_id_3_amount.Value);
                }
            }

            var newCharacterIds = new List<int>();

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var charId in characterRewardList)
                    {
                        var (isNew, itemId, amount) = await _characterService.giveCharacterAsync(personId, charId);

                        if (isNew == true)
                        {
                            newCharacterIds.Add(charId);
                        }

                        // 캐릭터가 조각으로 변환됨
                        if (itemId > 0 && amount > 0)
                        {
                            if (itemAmounts.ContainsKey(itemId))
                            {
                                itemAmounts[itemId] += amount;
                            }
                            else
                            {
                                itemAmounts[itemId] = amount;
                            }
                        }
                    }

                    // 지급
                    foreach (var item in itemAmounts)
                    {
                        await _itemService.giveItemAsync(personId, item.Key, item.Value);
                    }

                    if (totalToken > 0)
                    {
                        await _resourceService.addResourceAsync(personId, ResourceType.Token, totalToken);
                    }
                    if (totalPrism > 0)
                    {
                        await _resourceService.addResourceAsync(personId, ResourceType.Prism, totalPrism);
                    }
                    if (totalGachaResource > 0)
                    {
                        await _resourceService.addResourceAsync(personId, ResourceType.GachaResource, totalGachaResource);
                    }

                    // 우편 상태 변경
                    bool anyMailUpdated = false;
                    foreach (var mail in validMails)
                    {
                        // 로직상 방어
                        if (mail.is_receive) continue;

                        mail.is_receive = true;
                        mail.update_date = now;
                        anyMailUpdated = true;
                    }

                    if (!anyMailUpdated)
                    {
                        await transaction.RollbackAsync();
                        return emptyResult;
                    }

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                    await _personDataCacheService.deletePersonStatusAsync(personId);
                    _logger.LogInformation("User {PersonId} successfully received Mail", personId);
                    return await _rewardService.getObjectDisplayListByContentAsync(itemAmounts, newCharacterIds, totalPrism, totalToken, totalGachaResource);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to receive mail. User: {PersonId}", personId);
                    throw;
                }
            }
        }
    }
}