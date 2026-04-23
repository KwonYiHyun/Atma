using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ServerCore.Service;
using System.Security.Claims;

namespace Empen.Middleware
{
    public class RequestOneCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRedisLockService _redisLockService;

        public RequestOneCheckMiddleware(RequestDelegate next, IRedisLockService lockRepo)
        {
            _next = next;
            _redisLockService = lockRepo;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value?.ToLower() ?? "";

            // 예외 경로 처리
            if (path.StartsWith("/auth") ||
                path.StartsWith("/_") || // Blazor
                path.StartsWith("/app") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib") ||
                path.StartsWith("/icon") ||
                path.StartsWith("/images") ||
                path.StartsWith("/animeimage") ||
                path.EndsWith(".ico") ||
                path.EndsWith(".css") ||
                path.EndsWith(".js"))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var userClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userClaim == null)
            {
                await _next(context);
                return;
            }

            string personId = userClaim.Value;

            string? token = await _redisLockService.lockAsync(personId);

            if (token == null)
            {
                context.Response.StatusCode = 409; // 409(Conflict)
                await context.Response.WriteAsync("Previous request is still processing.");
                return;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                await _redisLockService.unLockAsync(personId, token);
            }
        }
    }

    public static class RequestOneMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestOneCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestOneCheckMiddleware>();
        }
    }
}