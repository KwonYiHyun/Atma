using Empen.Data;
using Empen.Service.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;
using System.Data;

namespace Empen.Service
{
    public class GachaService : IGachaService
    {
        private readonly PersonDbContext _personContext;
        private readonly IItemService _itemService;
        private readonly IResourceService _resourceService;
        private readonly ICharacterService _characterService;
        private readonly IRewardService _rewardService;
        private readonly IMasterDataCacheService _masterCacheService;
        private readonly ITimeService _timeService;
        private readonly ILogger<IGachaService> _logger;
        private readonly IAchievementService _achievementService;
        private readonly IPersonDataCacheService _personDataCacheService;

        public GachaService(MasterDbContext masterContext, PersonDbContext personContext, IItemService itemService, IResourceService resourceService, ICharacterService characterService, IRewardService rewardService, IMasterDataCacheService masterCacheService, ITimeService timeService, ILogger<IGachaService> logger, IAchievementService achievementService, IPersonDataCacheService personDataCacheService)
        {
            _personContext = personContext;
            _itemService = itemService;
            _resourceService = resourceService;
            _characterService = characterService;
            _rewardService = rewardService;
            _masterCacheService = masterCacheService;
            _timeService = timeService;
            _logger = logger;
            _achievementService = achievementService;
            _personDataCacheService = personDataCacheService;
        }

        public async Task<ICollection<GachaInfoDto>> getAllGachaAsync()
        {
            var gachaMap = await _masterCacheService.getGachaMapAsync();
            var now = await _timeService.getNowAsync();

            var gachas = gachaMap.Values
                .Where(g => (g.start_date <= now && g.end_date >= now) || (g.start_date <= now && g.end_date == null))
                .OrderBy(g => g.show_order)
                .Select(d => new GachaInfoDto
                {
                    gacha_id = d.gacha_id,
                    gacha_name = d.gacha_name,
                    gacha_type = d.gacha_type,
                    gacha_consume_value = d.gacha_consume_value,
                    gacha_image = d.gacha_image,
                    gacha_detail_image = d.gacha_detail_image,
                    gacha_detail_html = d.gacha_description,
                    gacha_point = d.gacha_point,
                    current_date = now,
                    start_date = d.start_date,
                    end_date = d.end_date
                })
                .ToList();

            return gachas;
        }
         
        public async Task<(ErrorCode, GachaInfoDto)> getGachaByIdAsync(int id)
        {
            var gachaMap = await _masterCacheService.getGachaMapAsync();

            if (!gachaMap.TryGetValue(id, out var gacha))
            {
                return (ErrorCode.GachaNotFound, null);
            }

            var dto = new GachaInfoDto
            {
                gacha_id = gacha.gacha_id,
                gacha_name = gacha.gacha_name,
                gacha_type = gacha.gacha_type,
                gacha_consume_value = gacha.gacha_consume_value,
                gacha_image = gacha.gacha_image,
                gacha_detail_image = gacha.gacha_detail_image,
                gacha_detail_html = gacha.gacha_description,
                gacha_point = gacha.gacha_point,
                start_date = gacha.start_date,
                end_date = gacha.end_date
            };

            return (ErrorCode.Success, dto);
        }

        public async Task<(ErrorCode, ICollection<GachaPlayInfoDto>)> playGacha(int personId, int gachaId, int execCount)
        {
            if (execCount != 1 && execCount != 10)
            {
                return (ErrorCode.InvalidExecCount, null);
            }

            bool canPlay = await canGacha(personId, gachaId, execCount);
            if (canPlay == false)
            {
                return (ErrorCode.NotEnoughFlim, null);
            }

            (ErrorCode status, ICollection<GachaPlayInfoDto> rewards) result;

            if (execCount == 1)
            {
                (result.status, result.rewards) = await playGachaExecAsync(personId, gachaId);
            }
            else
            {
                (result.status, result.rewards) = await playGachaExec10Async(personId, gachaId);
            }

            if (result.status == ErrorCode.Success)
            {
                await _achievementService.checkAndSetNewFlagAsync(personId, AchievementType.GACHA_PLAY);
            }

            return result;
        }

        public async Task<(ErrorCode, ICollection<GachaPlayInfoDto>)> playGachaExecAsync(int personId, int gachaId)
        {
            var gachaMap = await _masterCacheService.getGachaMapAsync();
            var gacha = gachaMap.Values
                .Where(g => g.gacha_id == gachaId)
                .FirstOrDefault();

            if (!gachaMap.TryGetValue(gachaId, out gacha))
            {
                return (ErrorCode.GachaNotFound, null);
            }

            if (gacha.gacha_lot_group_id == 0)
            {
                return (ErrorCode.GachaNotFound, null);
            }

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (gacha.gacha_type == 1)
                    {
                        // TODO: 스타트 가챠
                    }
                    else if (gacha.gacha_type == 2)
                    {
                        bool isConsumed = await _resourceService.consumeResourceAsync(personId, ResourceType.GachaResource, gacha.gacha_consume_value);

                        if (isConsumed == false)
                        {
                            await transaction.RollbackAsync();
                            return (ErrorCode.NotEnoughFlim, null);
                        }
                    } else if (gacha.gacha_type == 3)
                    {
                        // TODO: 티켓 전용 가챠
                    }
                    else
                    {
                        
                    }

                    // 천장 포인트
                    if (gacha.gacha_coupon_item_id > 0)
                    {
                        await _itemService.giveItemAsync(personId, gacha.gacha_coupon_item_id, 1);
                    }

                    var selectLots = await runGachaLogicOnly(gacha.gacha_lot_group_id, 1);

                    var resultList = await processGachaResults(personId, selectLots);

                    DateTime now = await _timeService.getNowAsync();
                    _personContext.person_gacha.Add(new person_gacha
                    {
                        person_id = personId,
                        gacha_id = gachaId,
                        gacha_count = 1,
                        insert_date = now,
                        update_date = now
                    });

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                    await _personDataCacheService.deletePersonStatusAsync(personId);
                    _logger.LogInformation("User {personId} successfully 1 play gacha {resultList}", personId, resultList);
                    return (ErrorCode.Success, resultList);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Exception from GachaService!");
                    return (ErrorCode.TransactionFailed, null);
                }
            }
        }

        public async Task<(ErrorCode, ICollection<GachaPlayInfoDto>)> playGachaExec10Async(int personId, int gachaId)
        {
            var gachaMap = await _masterCacheService.getGachaMapAsync();
            if (!gachaMap.TryGetValue(gachaId, out var gacha))
            {
                return (ErrorCode.GachaNotFound, null);
            }

            //if (gacha.gacha_exec_10_id == null)
            //{
            //    return (GachaResult.GachaNotFound, null);
            //}
            //var exec10 = gacha.gacha_exec_10;

            var gachaExec10Map = await _masterCacheService.getGachaExec10MapAsync();
            if (!gachaExec10Map.TryGetValue((int)gacha.gacha_exec_10_id, out var exec10))
            {
                return (ErrorCode.GachaNotFound, null);
            }

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (gacha.gacha_type == 1)
                    {
                        // TODO: 스타트 가챠
                    }
                    else if (gacha.gacha_type == 2)
                    {
                        int totalCost = gacha.gacha_consume_value * 10;

                        bool isConsumed = await _resourceService.consumeResourceAsync(personId, ResourceType.GachaResource, totalCost);

                        if (isConsumed == false)
                        {
                            await transaction.RollbackAsync();
                            return (ErrorCode.NotEnoughFlim, null);
                        }
                    }
                    else if (gacha.gacha_type == 3)
                    {
                        // TODO: 티켓 전용 가챠
                    }
                    else
                    {

                    }

                    // 천장 포인트
                    if (gacha.gacha_coupon_item_id > 0)
                    {
                        await _itemService.giveItemAsync(personId, gacha.gacha_coupon_item_id, 10);
                    }

                    List<master_gacha_lot> allSelectedLots = new List<master_gacha_lot>();

                    if (exec10.gacha_lot_count_1 > 0)
                    {
                        allSelectedLots.AddRange(await runGachaLogicOnly(exec10.gacha_lot_group_id_1, exec10.gacha_lot_count_1));
                    }
                    if (exec10.gacha_lot_count_2 > 0)
                    {
                        allSelectedLots.AddRange(await runGachaLogicOnly(exec10.gacha_lot_group_id_2, exec10.gacha_lot_count_2));
                    }
                    if (exec10.gacha_lot_count_3 > 0)
                    {
                        allSelectedLots.AddRange(await runGachaLogicOnly(exec10.gacha_lot_group_id_3, exec10.gacha_lot_count_3));
                    }

                    var resultList = await processGachaResults(personId, allSelectedLots);

                    DateTime now = await _timeService.getNowAsync();
                    _personContext.person_gacha.Add(new person_gacha
                    {
                        person_id = personId,
                        gacha_id = gachaId,
                        gacha_count = 10,
                        insert_date = now,
                        update_date = now
                    });

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                    await _personDataCacheService.deletePersonStatusAsync(personId);
                    _logger.LogInformation("User {personId} successfully 10 play gacha {resultList}", personId, resultList);
                    return (ErrorCode.Success, resultList);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Exception from GachaService!");
                    return (ErrorCode.TransactionFailed, null);
                }
            }
        }

        private async Task<List<master_gacha_lot>> runGachaLogicOnly(int gacha_lot_group_id, int count)
        {
            var lotMap = await _masterCacheService.getGachaLotMapAsync();

            // TODO: Dictionary<GroupId, List<Lot>> 형태로 캐싱
            var lots = lotMap.Values
                .Where(g => g.gacha_lot_group_id == gacha_lot_group_id)
                .ToList();

            if (lots.Count == 0)
            {
                return new List<master_gacha_lot>();
            }

            int totalWeight = lots.Sum(l => l.weight);
            List<master_gacha_lot> results = new List<master_gacha_lot>();

            for (int i = 0; i < count; i++)
            {
                int randNum = Random.Shared.Next(1, totalWeight + 1);
                int currentSum = 0;

                foreach (var lot in lots)
                {
                    currentSum += lot.weight;
                    if (randNum <= currentSum)
                    {
                        results.Add(lot);
                        break;
                    }
                }
            }

            return results;
        }

        private async Task<ICollection<GachaPlayInfoDto>> processGachaResults(int personId, List<master_gacha_lot> lots)
        {
            List<GachaPlayInfoDto> resultList = new List<GachaPlayInfoDto>();

            var gainedNewCharIds = new HashSet<int>();
            var gainedItemIds = new HashSet<int>();

            var tempResults = new List<(bool, int, int)>();

            var tempCharIds = new List<int>();

            foreach (var lot in lots)
            {
                var (isNew, itemId, amount) = await _characterService.giveCharacterAsync(personId, lot.gacha_character_id);

                if (isNew == true)
                {
                    gainedNewCharIds.Add(lot.gacha_character_id);
                    tempResults.Add((true, lot.gacha_character_id, 1));
                }
                else
                {
                    if (itemId > 0 && amount > 0)
                    {
                        await _itemService.giveItemAsync(personId, itemId, amount);


                        gainedItemIds.Add(itemId);
                        tempResults.Add((false, itemId, amount));
                    }
                    else
                    {
                        _logger.LogError("Gacha Error: Character {characterId} piece Item or Amount is invalid. person {personId}", lot.gacha_character_id, personId);
                    }
                }
            }

            var charMap = await _masterCacheService.getCharacterMapAsync();
            var itemMap = await _masterCacheService.getItemMapAsync();

            foreach (var (isNew, id, amount) in tempResults)
            {
                var playInfo = new GachaPlayInfoDto
                {
                    is_new = isNew,
                    obj = null
                };

                if (isNew)
                {
                    if(charMap.TryGetValue(id, out var mc))
                    {
                        playInfo.obj = _rewardService.createObjectDisplayByCharacter(mc);
                    }
                }
                else
                {
                    if (itemMap.TryGetValue(id, out var mi))
                    {
                        playInfo.obj = _rewardService.createObjectDisplayByItem(mi, amount);
                    }
                }

                if (playInfo.obj != null)
                {
                    resultList.Add(playInfo);
                }
            }

            return resultList;
        }

        public async Task<bool> canGacha(int personId, int gachaId, int execCount)
        {
            var gachaMap = await _masterCacheService.getGachaMapAsync();

            if (!gachaMap.TryGetValue(gachaId, out var gacha))
            {
                return false;
            }

            //var status = await _personContext.person_status
            //    .AsNoTracking()
            //    .Where(p => p.person_id == personId)
            //    .FirstOrDefaultAsync();

            var status = await _personDataCacheService.getPersonStatusAsync(personId);

            if (gacha == null || status == null)
            {
                return false;
            }

            if (gacha.gacha_type == 1)
            {
                // TODO: 스타트 가챠
                return true;
            }
            else if (gacha.gacha_type == 2)
            {
                int consume = gacha.gacha_consume_value;

                return consume * execCount <= status.film;
            }
            else if (gacha.gacha_type == 3)
            {
                // TODO: 티켓 전용 가챠
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
