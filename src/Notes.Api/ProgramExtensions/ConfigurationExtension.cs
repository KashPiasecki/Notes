using Notes.Api.Configuration;
using Notes.Api.Utility;

namespace Notes.Api.ProgramExtensions;

public static class ConfigurationExtension
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
        var configurationCollection = AssemblyLoader.GetTypes<IConfigurationInitializer>();
        foreach (var configuration in configurationCollection)   
        {
            serviceCollection.AddSingleton(configuration);
        }
    }
}
