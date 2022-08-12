using Notes.Infrastructure.ProgramExtensions;

// WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// ServiceCollection
var services = builder.Services;
var notesConfiguration = services.AddConfigurations(builder.Configuration);
services.AddControllers();
services.AddPostgresDatabase(notesConfiguration.Database);
services.AddSwagger(notesConfiguration.Swagger);

// WebApplication
var app = builder.Build(); 
app.MigrateDatabase();
app.UseSwagger(notesConfiguration.Swagger);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();