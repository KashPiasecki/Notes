using Microsoft.Extensions.Caching.Distributed;
using Notes.Application.Common.Interfaces;

namespace Notes.Infrastructure.Cache;

public class ResponseCacheService : IResponseCacheService
{
    private readonly IJsonConverterWrapper _json;
    private readonly IDistributedCache _distributedCache;


    public ResponseCacheService(IJsonConverterWrapper json, IDistributedCache distributedCache)
    {
        _json = json;
        _distributedCache = distributedCache;
    }

    public async Task CacheResponseAsync(string cacheKey, object? response, TimeSpan timeToLive)
    {
        if (response is null)
        {
            return;
        }

        var serializedResponse = _json.Serialize(response);
        await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = timeToLive
        });
    }

    public async Task<string> GetCachedResponseAsync(string cacheKey)
    {
        return await _distributedCache.GetStringAsync(cacheKey);
    }
}