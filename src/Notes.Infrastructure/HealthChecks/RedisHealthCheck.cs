using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notes.Domain.Configurations;
using StackExchange.Redis;

namespace Notes.Infrastructure.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RedisConfiguration _redisConfiguration;
    
    public RedisHealthCheck(RedisConfiguration redisConfiguration, IServiceProvider serviceProvider)
    {
        _redisConfiguration = redisConfiguration;
        _serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!_redisConfiguration.Enabled) return HealthCheckResult.Healthy("Redis disabled");
        using var scope = _serviceProvider.CreateScope();
        var connectionMultiplexer = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        try
        {
            var database = connectionMultiplexer.GetDatabase();
            await database.StringGetAsync("health");
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}