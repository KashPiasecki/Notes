using Notes.Application.Common.Interfaces;

namespace Notes.Infrastructure.Configuration;

public class DatabaseConfiguration : IConfigurationInitialize
{
    public string ConnectionString { get; init; }
}