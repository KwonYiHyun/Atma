using Empen.Data;
using Empen.Service;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Response;
using SharedData.Type;

namespace Empen.Controllers
{
    [Authorize]
    [Route("product")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // POST: product/buy/{productId}
        [HttpPost("buy/{productId}")]
        public async Task<ActionResult<GameResponse<string>>> buyProduct(int productId)
        {
            var errorCode = await _productService.buyProductSet(productId, CurrentPersonId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<string>(errorCode, "구매 실패"));
            }

            return Ok(new GameResponse<string>("구매 성공"));
        }

        // POST: product/prism/buy/{productId}
        [HttpPost("prism/buy/{productId}")]
        public async Task<ActionResult<GameResponse<string>>> buyProductPrism(int productId)
        {
            var errorCode = await _productService.buyProductSetPrism(productId, CurrentPersonId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<string>(errorCode, "구매 실패"));
            }

            return Ok(new GameResponse<string>("구매 성공"));
        }

        // POST: product/token/buy/{productId}
        [HttpPost("token/buy/{productId}")]
        public async Task<ActionResult<GameResponse<string>>> buyProductToken(int productId)
        {
            var errorCode = await _productService.buyProductSetToken(productId, CurrentPersonId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<string>(errorCode, "구매 실패"));
            }

            return Ok(new GameResponse<string>("구매 성공"));
        }

        // POST: product/piece/buy/{productId}
        [HttpPost("piece/buy/{productId}")]
        public async Task<ActionResult<GameResponse<string>>> buyProductPiece(int productId)
        {
            var errorCode = await _productService.buyProductSetPiece(productId, CurrentPersonId);

            if (errorCode != ErrorCode.Success)
            {
                return Ok(new GameResponse<string>(errorCode, "구매 실패"));
            }

            return Ok(new GameResponse<string>("구매 성공"));
        }

        // POST: product/get/person/all
        [HttpPost("get/person/all")]
        public async Task<ActionResult<GameResponse<ICollection<ProductSetDto>>>> getPersonAllProduct()
        {
            var products = await _productService.getAllProduct(CurrentPersonId);

            return Ok(new GameResponse<ICollection<ProductSetDto>>(products));
        }

        // POST: product/prism/get/person/all
        [HttpPost("prism/get/person/all")]
        public async Task<ActionResult<GameResponse<ICollection<ProductSetPrismDto>>>> getPersonAllProductSetPrism()
        {
            var products = await _productService.getAllProductSetPrism(CurrentPersonId);

            return Ok(new GameResponse<ICollection<ProductSetPrismDto>>(products));
        }

        // POST: product/token/get/person/all
        [HttpPost("token/get/person/all")]
        public async Task<ActionResult<GameResponse<ICollection<ProductSetTokenDto>>>> getPersonAllProductSetToken()
        {
            var products = await _productService.getAllProductSetToken(CurrentPersonId);

            return Ok(new GameResponse<ICollection<ProductSetTokenDto>>(products));
        }

        // POST: product/piece/get/person/all
        [HttpPost("piece/get/person/all")]
        public async Task<ActionResult<ICollection<ProductSetPieceDto>>> getPersonAllProductSetPiece()
        {
            var products = await _productService.getAllProductSetPiece(CurrentPersonId);

            return Ok(new GameResponse<ICollection<ProductSetPieceDto>>(products));
        }

        // POST: product/get/all
        [HttpPost("get/all")]
        public async Task<ActionResult<GameResponse<ICollection<ProductSetDto>>>> getAllProduct()
        {
            var products = await _productService.getAllProduct();

            return Ok(new GameResponse<ICollection<ProductSetDto>>(products));
        }

        // POST: product/prism/get/all
        [HttpPost("prism/get/all")]
        public async Task<ActionResult<GameResponse<ICollection<ProductSetPrismDto>>>> getAllProductSetPrism()
        {
            var products = await _productService.getAllProductSetPrism();

            return Ok(new GameResponse<ICollection<ProductSetPrismDto>>(products));
        }

        // POST: product/token/get/all
        [HttpPost("token/get/all")]
        public async Task<ActionResult<ICollection<ProductSetTokenDto>>> getAllProductSetToken()
        {
            var products = await _productService.getAllProductSetToken();

            return Ok(new GameResponse<ICollection<ProductSetTokenDto>>(products));
        }

        // POST: product/piece/get/all
        [HttpPost("piece/get/all")]
        public async Task<ActionResult<ICollection<ProductSetPieceDto>>> getAllProductSetPiece()
        {
            var products = await _productService.getAllProductSetPiece();

            return Ok(new GameResponse<ICollection<ProductSetPieceDto>>(products));
        }
    }
}
