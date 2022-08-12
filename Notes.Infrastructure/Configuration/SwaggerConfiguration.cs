using Notes.Application.Common.Interfaces;

namespace Notes.Infrastructure.Configuration;

public class SwaggerConfiguration : IConfigurationInitialize
{
    public string Name { get; init; }
    public string Version { get; init; }
    public string Developer { get; init; }
    public string Email { get; init; }
}
