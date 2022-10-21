using Microsoft.Extensions.Caching.Distributed;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Wrappers;

namespace Notes.Infrastructure.Cache;

public class ResponseCacheHandler : IResponseCacheHandler
{
    private readonly IJsonConverterWrapper _jsonConverterWrapper;
    private readonly IDistributedCacheWrapper _distributedCacheWrapper;
    
    public ResponseCacheHandler(IJsonConverterWrapper jsonConverterWrapper, IDistributedCacheWrapper distributedCacheWrapper)
    {
        _jsonConverterWrapper = jsonConverterWrapper;
        _distributedCacheWrapper = distributedCacheWrapper;
    }

    public async Task CacheResponseAsync(string cacheKey, object? response, TimeSpan timeToLive)
    {
        if (response is null) return;
        var serializedResponse = _jsonConverterWrapper.Serialize(response);
        await _distributedCacheWrapper.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeToLive
        });
    }

    public async Task<string> GetCachedResponseAsync(string cacheKey) => 
        await _distributedCacheWrapper.GetStringAsync(cacheKey);
    
}