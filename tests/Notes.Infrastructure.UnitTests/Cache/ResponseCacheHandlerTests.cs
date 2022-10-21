using Microsoft.Extensions.Caching.Distributed;
using Notes.Application.Common.Interfaces.Wrappers;
using Notes.Infrastructure.Cache;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Strings;
using TddXt.AnyRoot.Time;

namespace Notes.Infrastructure.UnitTests.Cache;

public class ResponseCacheHandlerTests
{
    [Test]
    public async Task CacheResponseAsync_NoResponse_DoesNothing()
    {
        // Arrange
        var jsonConverterWrapper = Substitute.For<IJsonConverterWrapper>();
        var distributedCacheWrapper = Substitute.For<IDistributedCacheWrapper>();

        var responseCacheHandler = new ResponseCacheHandler(jsonConverterWrapper, distributedCacheWrapper);

        // Act
        await responseCacheHandler.CacheResponseAsync(Any.String(), null, Any.TimeSpan());

        // Assert
        jsonConverterWrapper.DidNotReceiveWithAnyArgs().Serialize(Arg.Any<string>());
        await distributedCacheWrapper.DidNotReceiveWithAnyArgs()
            .SetStringAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DistributedCacheEntryOptions>());
    }

    [Test]
    public async Task CacheResponseAsync_ProperResponse_SavesToCache()
    {
        // Arrange
        var jsonConverterWrapper = Substitute.For<IJsonConverterWrapper>();
        var distributedCacheWrapper = Substitute.For<IDistributedCacheWrapper>();

        var responseCacheHandler = new ResponseCacheHandler(jsonConverterWrapper, distributedCacheWrapper);
        var response = Any.Object();
        var serializedResponse = Any.String();
        jsonConverterWrapper.Serialize(response).Returns(serializedResponse);
        var cacheKey = Any.String();

        // Act
        await responseCacheHandler.CacheResponseAsync(cacheKey, response, Any.TimeSpan());

        // Assert
        await distributedCacheWrapper.Received(1).SetStringAsync(cacheKey, serializedResponse, Arg.Any<DistributedCacheEntryOptions>());
    }

    [Test]
    public async Task CacheResponseAsync_Called_ReturnsCache()
    {
        // Arrange
        var distributedCacheWrapper = Substitute.For<IDistributedCacheWrapper>();

        var responseCacheHandler = new ResponseCacheHandler(Any.Instance<IJsonConverterWrapper>(), distributedCacheWrapper);
        var cacheKey = Any.String();

        // Act
        await responseCacheHandler.GetCachedResponseAsync(cacheKey);

        // Assert
        await distributedCacheWrapper.Received(1).GetStringAsync(cacheKey);
    }
}