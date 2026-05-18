using Empen.Data;
using Empen.Service.IService;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;
using System.Data;

namespace Empen.Service
{
    public class ProductService : IProductService
    {
        private readonly MasterDbContext _masterContext;
        private readonly PersonDbContext _personContext;
        private readonly IMailService _mailService;
        private readonly IRewardService _rewardService;
        private readonly IItemService _itemService;
        private readonly IResourceService _resourceService;
        private readonly ITimeService _timeService;
        private readonly ILogger<IProductService> _logger;
        private readonly IPersonDataCacheService _personDataCacheService;
        private readonly IMasterDataCacheService _masterDataCacheService;

        public ProductService(MasterDbContext masterContext, PersonDbContext personContext, IMailService mailService, IRewardService rewardService, IItemService itemService, IResourceService resourceService, ITimeService timeService, ILogger<IProductService> logger, IPersonDataCacheService personDataCacheService, IMasterDataCacheService masterDataCacheService)
        {
            _masterContext = masterContext;
            _personContext = personContext;
            _mailService = mailService;
            _rewardService = rewardService;
            _itemService = itemService;
            _resourceService = resourceService;
            _timeService = timeService;
            _logger = logger;
            _personDataCacheService = personDataCacheService;
            _masterDataCacheService = masterDataCacheService;
        }

        // 현금 상품
        public async Task<ErrorCode> buyProductSet(int productId, int personId)
        {
            _logger.LogInformation("User {PersonId} attempting to buy Product {ProductId}", personId, productId);

            var now = await _timeService.getNowAsync();

            var productInfo = await (from set in _masterContext.master_product_set.AsNoTracking()
                                     join prod in _masterContext.master_product.AsNoTracking()
                                     on set.product_id equals prod.product_id
                                     where set.product_id == productId
                                     select new
                                     {
                                         set.buy_upper_limit,
                                         set.start_date,
                                         set.end_date,
                                         prod.price,
                                         prod.product_name,
                                         Rewards = new List<int?> { set.reward_id_1, set.reward_id_2, set.reward_id_3 }
                                     }).FirstOrDefaultAsync();

            if (productInfo == null)
                return ErrorCode.ProductNotFound;

            if (now < productInfo.start_date || (productInfo.end_date != null && now > productInfo.end_date))
                return ErrorCode.NotInSalesPeriod;

            // 구매 제한 체크
            if (productInfo.buy_upper_limit > 0)
            {
                var currentCount = await _personContext.person_product
                    .AsNoTracking()
                    .CountAsync(p => p.person_id == personId && p.product_id == productId);

                if (currentCount >= productInfo.buy_upper_limit)
                    return ErrorCode.LimitExceeded;
            }

            // TODO: In-App Billing 검증 로직 (Receipt Validation)

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var history = new person_product
                    {
                        person_id = personId,
                        product_id = productId,
                        price = productInfo.price,
                        insert_date = now,
                        update_date = now
                    };
                    _personContext.person_product.Add(history);

                    foreach (var rewardId in productInfo.Rewards)
                    {
                        if (rewardId.HasValue && rewardId.Value > 0)
                        {
                            _mailService.sendMailOneAmountByPersonId(
                                personId,
                                Constant.PRODUCT_BUY_MAIL_TITLE,
                                string.Format(Constant.PRODUCT_BUY_MAIL_DES, productInfo.product_name),
                                rewardId.Value, null, null, now, now
                            );
                        }
                    }

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonStatusAsync(personId);
                    _logger.LogInformation("User {PersonId} successfully bought Product {ProductId}. Price: {Price}", personId, productId, productInfo.price);
                    return ErrorCode.Success;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to buy product. User: {PersonId}, Product: {ProductId}", personId, productId);
                    return ErrorCode.TransactionFailed;
                }
            }
        }

        public async Task<ICollection<ProductSetDto>> getAllProduct()
        {
            var now = await _timeService.getNowAsync();

            var rawData = await (from set in _masterContext.master_product_set.AsNoTracking()
                                 join prod in _masterContext.master_product.AsNoTracking()
                                 on set.product_id equals prod.product_id
                                 where (set.start_date <= now && (set.end_date == null || set.end_date >= now))
                                    && (prod.start_date <= now && (prod.end_date == null || prod.end_date >= now))
                                    && (set.start_date <= now && (set.end_date == null || set.end_date >= now))
                                 orderby set.show_order
                                 select new
                                 {
                                     set,
                                     prod
                                 }).ToListAsync();

            if (rawData.Count == 0)
            {
                return new List<ProductSetDto>();
            }

            // 보상 ID 수집
            var rewardIds = new HashSet<int>();
            foreach (var item in rawData)
            {
                if (item.set.reward_id_1 > 0)
                {
                    rewardIds.Add(item.set.reward_id_1);
                }
                if (item.set.reward_id_2.HasValue)
                {
                    rewardIds.Add(item.set.reward_id_2.Value);
                }
                if (item.set.reward_id_3.HasValue)
                {
                    rewardIds.Add(item.set.reward_id_3.Value);
                }
            }
            var rewardMap = await _rewardService.getObjectDisplayAsync(rewardIds.ToList());

            return rawData.Select(x => new ProductSetDto
            {
                product_set_id = x.set.product_set_id,
                product_id = x.set.product_id,

                product_info = new ProductInfoDto
                {
                    product_name = x.prod.product_name,
                    product_detail = x.prod.product_detail,
                    image = x.set.image,
                    price = x.prod.price,
                    price_type = PriceType.KRW,
                    buy_upper_limit = x.set.buy_upper_limit,
                    start_date = x.set.start_date,
                    end_date = x.set.end_date,
                    remaining_seconds = calculateRemainingSeconds(now, x.set.end_date),
                    reward_1 = rewardMap.GetValueOrDefault(x.set.reward_id_1),
                    reward_2 = x.set.reward_id_2.HasValue ? rewardMap.GetValueOrDefault(x.set.reward_id_2.Value) : null,
                    reward_3 = x.set.reward_id_3.HasValue ? rewardMap.GetValueOrDefault(x.set.reward_id_3.Value) : null
                }
            }).ToList();
        }

        public async Task<ICollection<ProductSetDto>> getAllProduct(int personId)
        {
            var products = await getAllProduct();
            if (products.Count == 0) return products;

            var productIds = products.Select(p => p.product_id).ToList();
            var buyCounts = await _personContext.person_product.AsNoTracking()
                .Where(p => p.person_id == personId && productIds.Contains(p.product_id))
                .GroupBy(p => p.product_id)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ProductId, x => x.Count);

            foreach (var p in products)
            {
                if (buyCounts.TryGetValue(p.product_id, out int count))
                {
                    p.product_info.buy_current_count = count;
                }
            }
            return products;
        }


        // 프리즘 상품
        public async Task<ErrorCode> buyProductSetPrism(int productSetPrismId, int personId)
        {
            _logger.LogInformation("User {PersonId} attempting to buy ProductSetPrism {ProductId}", personId, productSetPrismId);
            var now = await _timeService.getNowAsync();

            var productInfo = await _masterContext.master_product_set_prism
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.product_set_prism_id == productSetPrismId);

            if (productInfo == null)
                return ErrorCode.ProductNotFound;

            if (productInfo.start_date > now || (productInfo.end_date != null && productInfo.end_date < now))
                return ErrorCode.NotInSalesPeriod;

            if (productInfo.buy_upper_limit > 0)
            {
                var count = await _personContext.person_product_set_prism
                    .AsNoTracking()
                    .CountAsync(p => p.person_id == personId && p.product_set_prism_id == productSetPrismId);

                if (count >= productInfo.buy_upper_limit)
                    return ErrorCode.LimitExceeded;
            }

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    bool success = await _resourceService.consumeResourceAsync(personId, ResourceType.Prism, productInfo.price_prism);
                    if (!success)
                    {
                        return ErrorCode.NotEnoughCurrency;
                    }

                    _personContext.person_product_set_prism.Add(new person_product_set_prism
                    {
                        person_id = personId,
                        product_set_prism_id = productSetPrismId,
                        price = productInfo.price_prism,
                        insert_date = now,
                        update_date = now
                    });

                    var rewards = new List<int?> { productInfo.reward_id_1, productInfo.reward_id_2, productInfo.reward_id_3 };
                    foreach (var rewardId in rewards)
                    {
                        if (rewardId.HasValue && rewardId.Value > 0)
                        {
                            _mailService.sendMailOneAmountByPersonId(
                                personId,
                                Constant.PRODUCT_PRISM_BUY_MAIL_TITLE,
                                string.Format(Constant.PRODUCT_PRISM_BUY_MAIL_DES, productInfo.product_name),
                                rewardId.Value, null, null, now, now
                            );
                        }
                    }

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                    await _personDataCacheService.deletePersonStatusAsync(personId);
                    _logger.LogInformation("User {PersonId} successfully bought Product {productSetPrismId}. Price: {Price}", personId, productSetPrismId, productInfo.price_prism);
                    return ErrorCode.Success;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to buy product. User: {PersonId}, Product: {productSetPrismId}", personId, productSetPrismId);
                    return ErrorCode.TransactionFailed;
                }
            }
        }

        public async Task<ICollection<ProductSetPrismDto>> getAllProductSetPrism()
        {
            var now = await _timeService.getNowAsync();

            var rawList = await _masterContext.master_product_set_prism.AsNoTracking()
                .Where(p => p.start_date <= now && (p.end_date == null || p.end_date >= now))
                .OrderBy(m => m.show_order)
                .ToListAsync();

            if (rawList.Count == 0) return new List<ProductSetPrismDto>();

            var rewardIds = new HashSet<int>();
            foreach (var item in rawList)
            {
                if (item.reward_id_1 > 0)
                {
                    rewardIds.Add(item.reward_id_1);
                }
                if (item.reward_id_2.HasValue)
                {
                    rewardIds.Add(item.reward_id_2.Value);
                }
                if (item.reward_id_3.HasValue)
                {
                    rewardIds.Add(item.reward_id_3.Value);
                }
            }
            var rewardMap = await _rewardService.getObjectDisplayAsync(rewardIds.ToList());

            return rawList.Select(p => new ProductSetPrismDto
            {
                product_set_prism_id = p.product_set_prism_id,

                product_info = new ProductInfoDto
                {
                    product_name = p.product_name,
                    product_detail = p.product_detail,
                    image = p.image,
                    price = p.price_prism,
                    price_type = PriceType.PRISM,
                    buy_upper_limit = p.buy_upper_limit,
                    start_date = p.start_date,
                    end_date = p.end_date,
                    remaining_seconds = calculateRemainingSeconds(now, p.end_date),
                    reward_1 = rewardMap.GetValueOrDefault(p.reward_id_1),
                    reward_2 = p.reward_id_2.HasValue ? rewardMap.GetValueOrDefault(p.reward_id_2.Value) : null,
                    reward_3 = p.reward_id_3.HasValue ? rewardMap.GetValueOrDefault(p.reward_id_3.Value) : null
                }
            }).ToList();
        }

        public async Task<ICollection<ProductSetPrismDto>> getAllProductSetPrism(int personId)
        {
            var products = await getAllProductSetPrism();
            if (products.Count == 0)
            {
                return products;
            }

            var ids = products.Select(p => p.product_set_prism_id).ToList();
            var buyCounts = await _personContext.person_product_set_prism.AsNoTracking()
                .Where(p => p.person_id == personId && ids.Contains(p.product_set_prism_id))
                .GroupBy(p => p.product_set_prism_id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

            foreach (var p in products)
            {
                if (buyCounts.TryGetValue(p.product_set_prism_id, out int count))
                {
                    p.product_info.buy_current_count = count;
                }
            }
            return products;
        }


        // 토큰 상품
        public async Task<ErrorCode> buyProductSetToken(int productSetTokenId, int personId)
        {
            _logger.LogInformation("User {PersonId} attempting to buy ProductSetToken {ProductId}", personId, productSetTokenId);
            var now = await _timeService.getNowAsync();

            var productInfo = await _masterContext.master_product_set_token
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.product_set_token_id == productSetTokenId);

            if (productInfo == null)
            {
                return ErrorCode.ProductNotFound;
            }

            if (productInfo.start_date > now || (productInfo.end_date != null && productInfo.end_date < now))
            {
                return ErrorCode.NotInSalesPeriod;
            }

            if (productInfo.buy_upper_limit > 0)
            {
                var count = await _personContext.person_product_set_token
                    .AsNoTracking()
                    .CountAsync(p => p.person_id == personId && p.product_set_token_id == productSetTokenId);

                if (count >= productInfo.buy_upper_limit)
                {
                    return ErrorCode.LimitExceeded;
                }
            }

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    bool success = await _resourceService.consumeResourceAsync(personId, ResourceType.Token, productInfo.price_token);
                    if (!success)
                    {
                        return ErrorCode.NotEnoughCurrency;
                    }

                    _personContext.person_product_set_token.Add(new person_product_set_token
                    {
                        person_id = personId,
                        product_set_token_id = productSetTokenId,
                        price = productInfo.price_token,
                        insert_date = now,
                        update_date = now
                    });

                    var rewards = new List<int?> { productInfo.reward_id_1, productInfo.reward_id_2, productInfo.reward_id_3 };
                    foreach (var rewardId in rewards)
                    {
                        if (rewardId.HasValue && rewardId.Value > 0)
                        {
                            _mailService.sendMailOneAmountByPersonId(
                                personId,
                                Constant.PRODUCT_TOKEN_BUY_MAIL_TITLE,
                                string.Format(Constant.PRODUCT_TOKEN_BUY_MAIL_DES, productInfo.product_name),
                                rewardId.Value, null, null, now, now
                            );
                        }
                    }

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                    await _personDataCacheService.deletePersonStatusAsync(personId);
                    _logger.LogInformation("User {PersonId} successfully bought Product {productSetTokenId}. Price: {Price}", personId, productSetTokenId, productInfo.price_token);
                    return ErrorCode.Success;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to buy product. User: {PersonId}, Product: {productSetTokenId}", personId, productSetTokenId);
                    return ErrorCode.TransactionFailed;
                }
            }
        }

        public async Task<ICollection<ProductSetTokenDto>> getAllProductSetToken()
        {
            var now = await _timeService.getNowAsync();

            var rawList = await _masterContext.master_product_set_token.AsNoTracking()
                .Where(p => p.start_date <= now && (p.end_date == null || p.end_date >= now))
                .OrderBy(m => m.show_order)
                .ToListAsync();

            if (rawList.Count == 0)
            {
                return new List<ProductSetTokenDto>();
            }

            var rewardIds = new HashSet<int>();
            foreach (var item in rawList)
            {
                if (item.reward_id_1 > 0)
                {
                    rewardIds.Add(item.reward_id_1);
                }
                if (item.reward_id_2.HasValue)
                {
                    rewardIds.Add(item.reward_id_2.Value);
                }
                if (item.reward_id_3.HasValue)
                {
                    rewardIds.Add(item.reward_id_3.Value);
                }

                if (item.is_package == 1 && item.package_display_reward_id > 0)
                {
                    rewardIds.Add(item.package_display_reward_id);
                }
            }

            var rewardMap = await _rewardService.getObjectDisplayAsync(rewardIds.ToList());

            return rawList.Select(p => new ProductSetTokenDto
            {
                product_set_token_id = p.product_set_token_id,
                is_package = p.is_package,
                package_reward_display = rewardMap.GetValueOrDefault(p.package_display_reward_id),
                product_info = new ProductInfoDto
                {
                    product_name = p.product_name,
                    product_detail = p.product_detail,
                    image = p.image,
                    price = p.price_token,
                    price_type = PriceType.TOKEN,
                    buy_upper_limit = p.buy_upper_limit,
                    start_date = p.start_date,
                    end_date = p.end_date,
                    remaining_seconds = calculateRemainingSeconds(now, p.end_date),
                    reward_1 = rewardMap.GetValueOrDefault(p.reward_id_1),
                    reward_2 = p.reward_id_2.HasValue ? rewardMap.GetValueOrDefault(p.reward_id_2.Value) : null,
                    reward_3 = p.reward_id_3.HasValue ? rewardMap.GetValueOrDefault(p.reward_id_3.Value) : null
                }
            }).ToList();
        }

        public async Task<ICollection<ProductSetTokenDto>> getAllProductSetToken(int personId)
        {
            var products = await getAllProductSetToken();
            if (products.Count == 0) return products;

            var ids = products.Select(p => p.product_set_token_id).ToList();
            var buyCounts = await _personContext.person_product_set_token.AsNoTracking()
                .Where(p => p.person_id == personId && ids.Contains(p.product_set_token_id))
                .GroupBy(p => p.product_set_token_id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

            foreach (var p in products)
            {
                if (buyCounts.TryGetValue(p.product_set_token_id, out int count))
                {
                    p.product_info.buy_current_count = count;
                }
            }
            return products;
        }

        // 조각
        public async Task<ErrorCode> buyProductSetPiece(int productSetPieceId, int personId)
        {
            _logger.LogInformation("User {PersonId} attempting to buy ProductSetPiece {ProductId}", personId, productSetPieceId);
            var now = await _timeService.getNowAsync();

            var productInfo = await _masterContext.master_product_set_piece
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.product_set_piece_id == productSetPieceId);

            if (productInfo == null)
            {
                return ErrorCode.ProductNotFound;
            }

            // 기간 체크
            if (productInfo.start_date > now || (productInfo.end_date != null && productInfo.end_date < now))
            {
                return ErrorCode.NotInSalesPeriod;
            }

            if (productInfo.buy_upper_limit > 0)
            {
                var count = await _personContext.person_product_set_piece
                    .AsNoTracking()
                    .CountAsync(p => p.person_id == personId && p.product_set_piece_id == productSetPieceId);

                if (count >= productInfo.buy_upper_limit)
                {
                    return ErrorCode.LimitExceeded;
                }
            }

            // (재화 소모 + 이력 저장 + 보상 지급)
            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var useResult = await _itemService.useItemAsync(personId, productInfo.cost_item_id, productInfo.price_piece);
                    if (useResult != ErrorCode.Success)
                    {
                        return ErrorCode.NotEnoughCurrency;
                    }

                    var history = new person_product_set_piece
                    {
                        person_id = personId,
                        product_set_piece_id = productSetPieceId,
                        price = productInfo.price_piece,
                        insert_date = now,
                        update_date = now
                    };
                    _personContext.person_product_set_piece.Add(history);

                    var rewards = new List<int?> { productInfo.reward_id_1, productInfo.reward_id_2, productInfo.reward_id_3 };
                    foreach (var rewardId in rewards)
                    {
                        if (rewardId.HasValue && rewardId.Value > 0)
                        {
                            _mailService.sendMailOneAmountByPersonId(
                                personId,
                                Constant.PRODUCT_PIECE_BUY_MAIL_TITLE,
                                string.Format(Constant.PRODUCT_PIECE_BUY_MAIL_DES, productInfo.product_name),
                                rewardId.Value, null, null, now, now
                            );
                        }
                    }

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("User {PersonId} successfully bought Product {productSetPieceId}. Price: {Price}", personId, productSetPieceId, productInfo.price_piece);
                    await _personDataCacheService.deletePersonItemAsync(personId);
                    return ErrorCode.Success;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to buy product. User: {PersonId}, Product: {productSetPieceId}", personId, productSetPieceId);
                    return ErrorCode.TransactionFailed;
                }
            }
        }

        public async Task<ICollection<ProductSetPieceDto>> getAllProductSetPiece()
        {
            var now = await _timeService.getNowAsync();

            var productSetPieceAllMap = await _masterDataCacheService.getProductSetPieceMapAsync();

            var rawList = productSetPieceAllMap
                .Values
                .Where(p => p.start_date <= now && (p.end_date == null || p.end_date >= now))
                .OrderBy(m => m.show_order)
                .ToList();

            if (rawList.Count == 0)
            {
                return new List<ProductSetPieceDto>();
            }
            
            var allItemIds = new HashSet<int>();
            foreach (var item in rawList)
            {
                if (item.reward_id_1 > 0)
                {
                    allItemIds.Add(item.reward_id_1);
                }
                if (item.reward_id_2.HasValue)
                {
                    allItemIds.Add(item.reward_id_2.Value);
                }
                if (item.reward_id_3.HasValue)
                {
                    allItemIds.Add(item.reward_id_3.Value);
                }

                if (item.cost_item_id > 0)
                {
                    allItemIds.Add(item.cost_item_id);
                }
            }

            var displayMap = await _rewardService.getObjectDisplayAsync(allItemIds.ToList());

            // DTO 매핑
            return rawList.Select(p => new ProductSetPieceDto
            {
                product_set_piece_id = p.product_set_piece_id,

                cost_item = displayMap.GetValueOrDefault(p.cost_item_id),

                product_info = new ProductInfoDto
                {
                    product_name = p.product_name,
                    product_detail = p.product_detail,
                    image = p.image,
                    price = p.price_piece,
                    price_type = PriceType.PIECE,
                    buy_upper_limit = p.buy_upper_limit,
                    start_date = p.start_date,
                    end_date = p.end_date,
                    remaining_seconds = calculateRemainingSeconds(now, p.end_date),
                    reward_1 = displayMap.GetValueOrDefault(p.reward_id_1),
                    reward_2 = p.reward_id_2.HasValue ? displayMap.GetValueOrDefault(p.reward_id_2.Value) : null,
                    reward_3 = p.reward_id_3.HasValue ? displayMap.GetValueOrDefault(p.reward_id_3.Value) : null
                }
            }).ToList();
        }

        public async Task<ICollection<ProductSetPieceDto>> getAllProductSetPiece(int personId)
        {
            var products = await getAllProductSetPiece();

            if (products.Count == 0)
                return products;

            var ids = products.Select(p => p.product_set_piece_id).ToList();

            // 내 구매 횟수 조회
            var buyCounts = await _personContext.person_product_set_piece
                .AsNoTracking()
                .Where(p => p.person_id == personId && ids.Contains(p.product_set_piece_id))
                .GroupBy(p => p.product_set_piece_id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

            foreach (var p in products)
            {
                if (buyCounts.TryGetValue(p.product_set_piece_id, out int count))
                {
                    p.product_info.buy_current_count = count;
                }
            }

            return products;
        }

        private long calculateRemainingSeconds(DateTime now, DateTime? endDate)
        {
            if (endDate == null)
            {
                return -1;
            }

            TimeSpan diff = endDate.Value - now;
            long seconds = (long)diff.TotalSeconds;

            return seconds > 0 ? seconds : 0;
        }
    }
}