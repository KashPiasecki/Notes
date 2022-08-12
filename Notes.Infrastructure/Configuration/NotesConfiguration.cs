namespace Notes.Infrastructure.Configuration;

public class NotesConfiguration
{
    public SwaggerConfiguration Swagger { get; init; }
    public DatabaseConfiguration Database { get; init; }
    
}