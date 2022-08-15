using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Infrastructure.Configuration;
using Notes.Infrastructure.Utility;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureConfigurations
{
    private const string SectionName = "NotesConfiguration";
    
    public static NotesConfiguration AddConfigurations(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var notesConfiguration = configuration.GetSection(SectionName).Get<NotesConfiguration>();
        RegisterSubConfigurations(serviceCollection);
        return notesConfiguration;
    }

    private static void RegisterSubConfigurations(IServiceCollection serviceCollection)
    {
        var configurationCollection = AssemblyLoader.GetTypes<IConfigurationInitialize>();
        foreach (var configuration in configurationCollection)   
        {
            serviceCollection.AddSingleton(configuration);
        }
    }
}
