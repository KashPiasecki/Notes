using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notes.Domain.Configurations;

namespace Notes.Infrastructure.Configuration;

public static class ConfigureConfigurations
{
    private const string SectionName = "NotesConfiguration";

    public static NotesConfiguration AddConfigurations(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var notesConfiguration = configuration.GetSection(SectionName).Get<NotesConfiguration>();
        serviceCollection.AddSingleton(notesConfiguration.Database);
        serviceCollection.AddSingleton(notesConfiguration.Swagger);
        serviceCollection.AddSingleton(notesConfiguration.JwtSettings);
        return notesConfiguration;
    }
}