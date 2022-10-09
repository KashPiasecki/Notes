using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notes.Api.HealthChecks;
using Notes.Application.HealthChecks;
using Notes.Domain.Contracts;
using Notes.Infrastructure.Persistence;

namespace Notes.Api.ConfigureServices;

public static class ConfigureHealthCheck
{
    public static void AddHealthCheck(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHealthChecks()
            .AddDbContextCheck<DataContext>()
            .AddCheck<RedisHealthCheck>("redis");
    }

    public static void UseHealthCheck(this WebApplication webApplication)
    {
        webApplication.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new HealthCheckResponse
                {
                    Status = report.Status.ToString(),
                    Checks = report.Entries.Select(x => new HealthCheck
                    {
                        Component = x.Key,
                        Status = x.Value.Status.ToString(),
                        Description = x.Value.Description ?? string.Empty
                    }),
                    Duration = report.TotalDuration
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        });
    }
}