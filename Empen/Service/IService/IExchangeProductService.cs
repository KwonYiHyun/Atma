using SharedData.Dto;

namespace Empen.Service.IService
{
    public interface IExchangeProductService
    {
        Task<ICollection<ExchangeProductSetDto>> getExchangeProductSetList();
        Task<ICollection<ExchangeProductDto>> getExchangeProductList(int personId, int exchangeProductSetId);
    }
}
