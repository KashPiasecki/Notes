namespace Notes.Domain.Configurations;

public class RedisConfiguration
{
    public bool Enabled { get; init; }
    public string ConnectionString { get; init; }
}