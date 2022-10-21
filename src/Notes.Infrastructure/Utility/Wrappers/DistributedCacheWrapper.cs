using Microsoft.Extensions.Caching.Distributed;
using Notes.Application.Common.Interfaces.Wrappers;

namespace Notes.Infrastructure.Utility.Wrappers;

public class DistributedCacheWrapper : IDistributedCacheWrapper
{
    private readonly IDistributedCache _distributedCache;

    public DistributedCacheWrapper(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options) =>
        _distributedCache.SetStringAsync(key, value, options);

    public Task<string> GetStringAsync(string cacheKey) =>
        _distributedCache.GetStringAsync(cacheKey);
}