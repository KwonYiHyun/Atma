using Empen.Data;
using SharedData.Dto;
using SharedData.Type;

namespace Empen.Service.IService
{
    public interface IProductService
    {
        Task<ICollection<ProductSetDto>> getAllProduct();
        Task<ICollection<ProductSetPrismDto>> getAllProductSetPrism();
        Task<ICollection<ProductSetTokenDto>> getAllProductSetToken();
        Task<ICollection<ProductSetPieceDto>> getAllProductSetPiece();
        Task<ICollection<ProductSetDto>> getAllProduct(int personId);
        Task<ICollection<ProductSetPrismDto>> getAllProductSetPrism(int personId);
        Task<ICollection<ProductSetTokenDto>> getAllProductSetToken(int personId);
        Task<ICollection<ProductSetPieceDto>> getAllProductSetPiece(int personId);
        Task<ErrorCode> buyProductSet(int productId, int personId);
        Task<ErrorCode> buyProductSetPrism(int productSetPrismId, int personId);
        Task<ErrorCode> buyProductSetToken(int productSetTokenId, int personId);
        Task<ErrorCode> buyProductSetPiece(int productSetPieceId, int personId);
    }
}
