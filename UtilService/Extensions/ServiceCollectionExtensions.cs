using CloudStructures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config.GetConnectionString("RedisConnection");

            var redisConfig = new RedisConfig("Global", connectionString);

            var connection = new RedisConnection(redisConfig);
            services.AddSingleton(connection);

            services.AddSingleton<IRedisService, RedisService>();
            services.AddSingleton<IRedisLockService, RedisLockService>();

            services.AddScoped<ITimeService, TimeService>();
        }
    }
}
