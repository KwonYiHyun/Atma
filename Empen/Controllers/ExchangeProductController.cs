using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Empen.Controllers
{
    [Authorize]
    [Route("exchangeproduct")]
    [ApiController]
    public class ExchangeProductController : BaseController
    {
    }
}
