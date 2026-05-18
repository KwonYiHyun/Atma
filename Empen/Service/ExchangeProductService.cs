using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using SharedData.Dto;

namespace Empen.Service
{
    //public class ExchangeProductService : IExchangeProductService
    //{
    //    private readonly MasterDbContext _masterContext;
    //    private readonly PersonDbContext _personContext;

    //    public ExchangeProductService(MasterDbContext masterDbContext, PersonDbContext personContext)
    //    {
    //        _masterContext = masterDbContext;
    //        _personContext = personContext;
    //    }

    //    public async Task<ICollection<ExchangeProductDto>> getExchangeProductList(int personId, int exchangeProductSetId)
    //    {
    //        DateTime now = DateTime.Now;

    //        var buyCounts = await _personContext.person_exchange_product
    //            .AsNoTracking()
    //            .Where(e => e.person_id == personId)
    //            .GroupBy(e => e.exchange_product_id)
    //            .Select(e => new { exchange_product_id = e.Key, Count = e.Count() })
    //            .ToDictionaryAsync(e => e.exchange_product_id, e => e.Count);

    //        var list = await _masterContext.master_exchange_product
    //            .AsNoTracking()
    //            .Where(e => e.start_date >= now && (e.end_date == null || e.end_date >= now))
    //            .Select(e => new ExchangeProductDto
    //            {
    //                exchange_product_id = e.exchange_product_id,
    //                buy_upper_limit = e.buy_upper_limit,
    //                require_amount = e.require_amount,
    //                require_item_id = e.require_item_id,
    //                reward_id = e.reward_id,
    //                image = e.image,

    //                pay_count = buyCounts[e.exchange_product_id]
    //            })
    //            .ToListAsync();

    //        return list;
    //    }

    //    public async Task<ICollection<ExchangeProductSetDto>> getExchangeProductSetList()
    //    {
    //        DateTime now = DateTime.Now;

    //        var list = await _masterContext.master_exchange_product_set
    //            .AsNoTracking()
    //            .Where(e => e.start_date >= now && (e.end_date == null || e.end_date >= now))
    //            .Select(e => new ExchangeProductSetDto
    //            {
    //                exchange_product_set_id = e.exchange_product_set_id,
    //                sales_start_date = e.sales_start_date,
    //                sales_end_date = e.sales_end_date,
    //                title = e.title,
    //                plate_type = e.plate_type,
    //                button_image = e.button_image
    //            })
    //            .ToListAsync();

    //        return list;
    //    }
    //}
}
