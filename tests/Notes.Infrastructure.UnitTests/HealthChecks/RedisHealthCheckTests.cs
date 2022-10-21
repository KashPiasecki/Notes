using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notes.Domain.Configurations;
using Notes.Infrastructure.HealthChecks;
using NSubstitute.ExceptionExtensions;
using StackExchange.Redis;
using TddXt.AnyRoot.Builder;

namespace Notes.Infrastructure.UnitTests.HealthChecks;

public class RedisHealthChecksTests
{
    [Test]
    public async Task CheckHealthAsync_WithDisabledConfiguration_ReturnsDisabled()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var redisConfiguration = Any.Instance<RedisConfiguration>()
            .WithProperty(x => x.Enabled, false);
        
        var redisHealthCheck = new RedisHealthCheck(redisConfiguration, serviceProvider);
        var healthCheckContext = Any.Instance<HealthCheckContext>();

        // Act
        var result = await redisHealthCheck.CheckHealthAsync(healthCheckContext);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("Redis disabled");
    }
    
    [Test]
    public async Task CheckHealthAsync_WithHealthyRedis_ReturnsHealthy()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var serviceScopedFactory = Substitute.For<IServiceScopeFactory>();
        var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
        serviceScopedFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(serviceProvider);
        serviceProvider.GetService<IServiceScopeFactory>().Returns(serviceScopedFactory);
        serviceProvider.GetService<IConnectionMultiplexer>().Returns(connectionMultiplexer);
        var redisConfiguration = Any.Instance<RedisConfiguration>()
            .WithProperty(x => x.Enabled, true);
        
        var redisHealthCheck = new RedisHealthCheck(redisConfiguration, serviceProvider);
        var healthCheckContext = Any.Instance<HealthCheckContext>();
        
        // Act
        var result = await redisHealthCheck.CheckHealthAsync(healthCheckContext);
        
        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }
    
    [Test]
    public async Task CheckHealthAsync_WithUnhealthyRedis_ReturnsUnhealthy()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var serviceScopedFactory = Substitute.For<IServiceScopeFactory>();
        var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
        serviceScopedFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(serviceProvider);
        serviceProvider.GetService<IServiceScopeFactory>().Returns(serviceScopedFactory);
        serviceProvider.GetService<IConnectionMultiplexer>().Returns(connectionMultiplexer);
        var redisConfiguration = Any.Instance<RedisConfiguration>()
            .WithProperty(x => x.Enabled, true);
        
        var redisHealthCheck = new RedisHealthCheck(redisConfiguration, serviceProvider);
        var healthCheckContext = Any.Instance<HealthCheckContext>();
        connectionMultiplexer.GetDatabase()
            .Throws(new RedisConnectionException(ConnectionFailureType.ConnectionDisposed, "test"));

        // Act
        var result = await redisHealthCheck.CheckHealthAsync(healthCheckContext);
    
        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }
}