using Microsoft.Extensions.Caching.Distributed;

namespace Notes.Application.Common.Interfaces.Wrappers;

public interface IDistributedCacheWrapper
{
    Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options);
    Task<string> GetStringAsync(string cacheKey);
}