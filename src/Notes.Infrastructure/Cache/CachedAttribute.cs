using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Configurations;
using Notes.Infrastructure.Utility.Extensions;

namespace Notes.Infrastructure.Cache;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CachedAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _timeToLiveSeconds;

    public CachedAttribute(int timeToLiveSeconds)
    {
        _timeToLiveSeconds = timeToLiveSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CachedAttribute>>();
        var redisConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();
        if (!redisConfiguration.Enabled)
        {
            await next();
            return;
        }

        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext);
        var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);
        logger.LogInformation("Accessing redis cache");

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            var contentResult = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200
            };

            logger.LogInformation("Response retrieved from the cache");
            context.Result = contentResult;
            return;
        }

        var executedContext = await next();

        if (executedContext.Result is OkObjectResult okObjectResult)
        {
            await cacheService.CacheResponseAsync(cacheKey,
                okObjectResult.Value ?? throw new ArgumentNullException(nameof(cachedResponse), "Service returned empty ok result"),
                TimeSpan.FromSeconds(_timeToLiveSeconds));
            logger.LogInformation("No response in the cache. Saving new response to redis");
        }
    }

    private static string GenerateCacheKeyFromRequest(HttpContext contextHttp)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{contextHttp.Request.Path}{contextHttp.GetUserId()}");
        foreach (var (key, value) in contextHttp.Request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"{key}:{value}");
        }
        
        return keyBuilder.ToString();
    }
}