using Empen.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ServerCore.Service;
using SharedData.Dto;
using System.Security.Claims;
using System.Text.Json;
using System.Web;

namespace Empen.Filter
{
    public class AchievementCheckFilter : IAsyncActionFilter
    {
        private readonly IRedisService _redisService;

        public AchievementCheckFilter(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            int personId = GetCurrentUserId(context.HttpContext);

            if (personId > 0)
            {
                string key = $"Achievement:Pending:{personId}";
                List<AchievementSuccessInfo> pending = await _redisService.popAllListItemsAsync<AchievementSuccessInfo>(key);

                if (pending != null && pending.Count > 0)
                {
                    string finalJson = JsonConvert.SerializeObject(pending);

                    string encodedValue = HttpUtility.UrlEncode(finalJson);

                    if (!context.HttpContext.Response.HasStarted)
                    {
                        context.HttpContext.Response.Headers.Add("X-New-Achievement-Titles", encodedValue);
                    }
                }
            }
        }

        private int GetCurrentUserId(HttpContext context)
        {
            var idClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            int personId = 0;

            if (idClaim != null && int.TryParse(idClaim.Value, out personId))
            {
                return personId;
            }
            else
            {
                return 0;
            }
        }
    }
}
