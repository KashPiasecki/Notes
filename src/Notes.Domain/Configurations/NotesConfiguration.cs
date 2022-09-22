namespace Notes.Domain.Configurations;

public class NotesConfiguration
{
    public SwaggerConfiguration Swagger { get; init; }
    public DatabaseConfiguration Database { get; init; }
    public JwtConfiguration JwtSettings { get; init; }
    
    
}