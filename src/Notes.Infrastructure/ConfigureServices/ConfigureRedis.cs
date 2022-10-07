using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Configurations;
using Notes.Infrastructure.Cache;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureRedis
{
    public static void AddRedis(this IServiceCollection serviceCollection, RedisConfiguration redisConfiguration)
    {
        if (redisConfiguration.Enabled)
        {
            serviceCollection.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfiguration.ConnectionString;
            });
            serviceCollection.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}