using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ServerCore.Service; // IRedisService가 있는 네임스페이스
using SharedData.Response;
using SharedData.Type;
using System.Text.Json;

namespace ServerCore.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        private const string MAINTENANCE_KEY = "server:maintenance";

        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRedisService redisService)
        {
            var path = context.Request.Path.Value;

            // 예외 처리
            if (!string.IsNullOrEmpty(path) && (path.StartsWith("/admin") || path.StartsWith("/health")))
            {
                await _next(context);
                return;
            }

            string maintenanceStatus = await redisService.getAsync<string>(MAINTENANCE_KEY);

            if (!string.IsNullOrEmpty(maintenanceStatus))
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";

                var response = new GameResponse<string>(ErrorCode.ServerMaintenance, "현재 서버 점검 중입니다.");

                string jsonString = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonString);
                return;
            }

            await _next(context);
        }
    }

    public static class MaintenanceMiddlewareExtensions
    {
        public static IApplicationBuilder UseMaintenanceCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MaintenanceMiddleware>();
        }
    }
}