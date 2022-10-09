using Notes.Domain.Contracts;

namespace Notes.Application.HealthChecks;

public class HealthCheckResponse
{
    public string Status { get; init; }
    public IEnumerable<HealthCheck> Checks { get; init; }
    public TimeSpan Duration { get; init; }
}