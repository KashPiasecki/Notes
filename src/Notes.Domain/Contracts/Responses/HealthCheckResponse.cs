using Notes.Domain.Contracts.Entities;

namespace Notes.Domain.Contracts.Responses;

public class HealthCheckResponse
{
    public string Status { get; init; }
    public IEnumerable<HealthCheck> Checks { get; init; }
    public TimeSpan Duration { get; init; }
}