using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Empen.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string API_KEY_HEADER_NAME = "X-Admin-Api-Key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(new { errorCode = "MissingApiKey", message = "API 키가 누락되었습니다." });
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var realApiKey = configuration.GetValue<string>("AdminSettings:ApiKey");

            if (!realApiKey.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(new { errorCode = "InvalidApiKey", message = "유효하지 않은 API 키입니다." });
                return;
            }

            await next();
        }
    }
}
