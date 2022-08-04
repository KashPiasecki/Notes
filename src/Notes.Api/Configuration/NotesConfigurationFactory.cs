namespace Notes.Api.Configuration;

public static class NotesConfigurationFactory
{
    private const string SectionName = "NotesConfiguration";

    public static NotesConfiguration Get(IConfiguration configuration)
    {
        var notesConfiguration = new NotesConfiguration();
        configuration.GetSection(SectionName).Bind(notesConfiguration);
        return notesConfiguration;
    } 
}