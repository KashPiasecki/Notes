namespace Notes.Application.Common.Interfaces.Handlers;

public interface IResponseCacheHandler
{
    Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
    Task<string> GetCachedResponseAsync(string cacheKey);
}