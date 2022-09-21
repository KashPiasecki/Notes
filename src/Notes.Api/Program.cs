using Notes.Application.ConfigureServices;
using Notes.Infrastructure.ConfigureServices;

// WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// ServiceCollection - Api
var services = builder.Services;
var notesConfiguration = services.AddConfigurations(builder.Configuration);
services.AddControllers();

// ServiceCollection - Application
services.AddMediatR();

// ServiceCollection - Infrastructure
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