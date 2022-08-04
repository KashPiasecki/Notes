using Notes.Api.ProgramExtensions;

namespace Notes.Api.Configuration;

public class SwaggerConfiguration : IConfigurationInitializer
{
    public string Name { get; init; }
    public string Version { get; init; }
    public string Developer { get; init; }
    public string Email { get; init; }
}
