using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;

namespace Empen.Service
{
    public class ItemService : IItemService
    {
        private readonly PersonDbContext _personContext;
        private readonly MasterDbContext _masterContext;
        private readonly IResourceService _resourceService;
        private readonly ITimeService _timeService;
        private readonly IMasterDataCacheService _masterCacheService;
        private readonly ILogger<IItemService> _logger;
        private readonly IRedisService _redisService;
        private readonly IPersonDataCacheService _personDataCacheService;

        public ItemService(PersonDbContext personContext, MasterDbContext masterContext, IResourceService resourceService, ITimeService timeService, IMasterDataCacheService masterCacheService, ILogger<IItemService> logger, IRedisService redisService, IPersonDataCacheService personDataCacheService)
        {
            _personContext = personContext;
            _masterContext = masterContext;
            _resourceService = resourceService;
            _timeService = timeService;
            _masterCacheService = masterCacheService;
            _logger = logger;
            _redisService = redisService;
            _personDataCacheService = personDataCacheService;
        }

        public async Task commitAsync()
        {
            await _personContext.SaveChangesAsync();
        }

        public async Task<ICollection<CommonItemInfoDto>> getAllItem(int personId)
        {
            DateTime now = await _timeService.getNowAsync();
            var emptyResult = new List<CommonItemInfoDto>();

            //var personItems = await _personContext.person_item
            //    .AsNoTracking()
            //    .Where(pi => pi.person_id == personId && pi.amount > 0)
            //    .ToListAsync();

            var personItems = await _personDataCacheService.getPersonItemAsync(personId);

            if (personItems.Count == 0)
            {
                return emptyResult;
            }

            var masterMap = await _masterCacheService.getItemMapAsync();

            var dtoList = personItems
                .Select(pi =>
                {
                    if (!masterMap.TryGetValue(pi.item_id, out var mi))
                    {
                        return null;
                    }

                    if (mi.expire_date != null && mi.expire_date < now)
                    {
                        return null;
                    }

                    return new CommonItemInfoDto
                    {
                        id = mi.item_id,
                        name = mi.item_name,
                        description = mi.item_description,
                        image = mi.item_image_name,
                        amount = pi.amount,

                        item_type = mi.item_type,
                        tab_type = mi.tab_type,
                        item_param_1 = mi.item_param_1,
                        item_param_2 = mi.item_param_2,
                        expire_date = mi.expire_date,

                        usable = mi.item_type.isUsable()
                    };
                })
                .Where(dto => dto != null)
                .ToList();

            return dtoList;
        }

        public async Task<ErrorCode> giveItemAndSaveAsync(int personId, int itemId, int amount)
        {
            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await giveItemAsync(personId, itemId, amount);

                    if (result != ErrorCode.Success)
                    {
                        await transaction.RollbackAsync();
                        return result;
                    }

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                    return ErrorCode.Success;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed giveItemAndSaveAsync PersonId {personId} ItemId {itemId}", personId, itemId);
                    return ErrorCode.TransactionFailed;
                }
            }
        }

        public async Task<ErrorCode> giveItemAsync(int personId, int itemId, int amount)
        {
            if (amount <= 0)
            {
                return ErrorCode.Success;
            }

            DateTime now = await _timeService.getNowAsync();

            try
            {
                FormattableString sql = $@"
                    MERGE INTO person_item WITH (HOLDLOCK) AS target
                    USING (SELECT {personId} AS person_id, {itemId} AS item_id) AS source
                    ON (target.person_id = source.person_id AND target.item_id = source.item_id)

                    WHEN MATCHED THEN
                        UPDATE SET 
                            target.amount = target.amount + {amount},
                            target.update_date = {now}

                    WHEN NOT MATCHED AND ({amount} > 0) THEN
                        INSERT (person_id, item_id, amount, insert_date, update_date)
                        VALUES ({personId}, {itemId}, {amount}, {now}, {now});
                ";

                int affectedRows = await _personContext.Database.ExecuteSqlInterpolatedAsync(sql);

                if (affectedRows == 0)
                {
                    return ErrorCode.DataNotFound;
                }

                var history = new person_item_history
                {
                    person_id = personId,
                    item_id = itemId,
                    amount = amount,
                    insert_date = now,
                    update_date = now
                };
                _personContext.person_item_history.Add(history);

                return ErrorCode.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild RawSQL in giveItemAsync");
                return ErrorCode.TransactionFailed;
            }
        }

        public async Task<ErrorCode> useItemAndSaveAsync(int personId, int itemId, int amount)
        {
            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await useItemAsync(personId, itemId, amount);

                    if (result != ErrorCode.Success)
                    {
                        await transaction.RollbackAsync();
                        return result;
                    }

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                    return ErrorCode.Success;
                }
                catch(Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed useItemAndSaveAsync PersonId {personId} ItemId {itemId}", personId, itemId);
                    return ErrorCode.TransactionFailed;
                }
            }
        }

        public async Task<ErrorCode> useItemAsync(int personId, int itemId, int amount)
        {
            var personItems = await _personDataCacheService.getPersonItemAsync(personId);
            var targetItem = personItems.FirstOrDefault(i => i.item_id == itemId);

            if (targetItem == null || targetItem.amount < amount)
            {
                return ErrorCode.ItemNotFound;
            }

            var now = await _timeService.getNowAsync();
            var itemMap = await _masterCacheService.getItemMapAsync();

            if (!itemMap.TryGetValue(itemId, out var masterItem))
            {
                return ErrorCode.ItemNotFound;
            }

            if (masterItem.expire_date != null && masterItem.expire_date < now)
            {
                //await _personContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM person_item WHERE person_id = {personId} AND item_id = {itemId}");
                await _personContext.person_item.Where(p => p.person_id == personId && p.item_id == itemId).ExecuteDeleteAsync();
                return ErrorCode.Expired;
            }

            bool effectApplied = false;

            switch (masterItem.item_type)
            {
                case (int)ItemType.GACHA_TICKET:
                    // int recoverAmount = (masterItem.item_param_1 ?? 0) * amount;
                    // await _resourceService.addResourceAsync(personId, ResourceType.ActivePoint, recoverAmount);
                    effectApplied = true;
                    break;
                case (int)ItemType.GACHA_POINT:

                    effectApplied = true;
                    break;
                case (int)ItemType.CHARACTER_PIECE:

                    effectApplied = true;
                    break;
                case (int)ItemType.CHARACTER_REINFORCEMENT_MATERIAL:

                    effectApplied = true;
                    break;
                default:
                    return ErrorCode.UnknownType;
            }

            if (!effectApplied)
            {
                return ErrorCode.EffectFailed;
            }

            //FormattableString sql = $@"
            //    UPDATE person_item
            //    SET amount = amount - {amount},
            //        update_date = {now}
            //    WHERE person_id = {personId} 
            //      AND item_id = {itemId} 
            //      AND amount >= {amount};
            //";

            //int affectedRows = await _personContext.Database.ExecuteSqlInterpolatedAsync(sql);

            //if (affectedRows == 0)
            //{
            //    return ErrorCode.NotEnoughAmount;
            //}

            //await _personContext.Database.ExecuteSqlInterpolatedAsync(
            //    $"DELETE FROM person_item WHERE person_id = {personId} AND item_id = {itemId} AND amount <= 0");

            int affectedRows = await _personContext.person_item
                .Where(i => i.person_id == personId && i.item_id == itemId && i.amount >= amount)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.amount, i => i.amount - amount).SetProperty(i => i.update_date, now));

            if (affectedRows == 0)
            {
                return ErrorCode.NotEnoughAmount;
            }

            await _personContext.person_item
                .Where(i => i.person_id == personId && i.item_id == itemId && i.amount <= 0)
                .ExecuteDeleteAsync();

            var history = new person_item_history
            {
                person_id = personId,
                item_id = itemId,
                amount = -amount,
                insert_date = now,
                update_date = now
            };
            _personContext.person_item_history.Add(history);

            return ErrorCode.Success;
        }
    }
}
