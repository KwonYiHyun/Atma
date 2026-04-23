using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Empen.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int CurrentPersonId
        {
            get
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (idClaim != null && int.TryParse(idClaim.Value, out int personId))
                {
                    return personId;
                }

                throw new UnauthorizedAccessException("Invalid Token: No PersonId found.");
            }
        }
    }
}
