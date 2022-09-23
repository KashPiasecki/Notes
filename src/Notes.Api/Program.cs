using Notes.Application.ConfigureServices;
using Notes.Infrastructure.ConfigureServices;
using Notes.Infrastructure.ConfigureServices.Swagger;

// WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// ServiceCollection - Api
var services = builder.Services;
var notesConfiguration = services.AddConfigurations(builder.Configuration);
services.AddControllers();

// ServiceCollection - Application
services.AddMediatR();

// ServiceCollection - Infrastructure
services.AddServices();
services.AddPostgresDatabase(notesConfiguration.Database);
services.AddIdentity(notesConfiguration.JwtSettings.Secret);
services.AddSwagger(notesConfiguration.Swagger);

// WebApplication
var app = builder.Build(); 
app.MigrateDatabase();
app.UseSwagger(notesConfiguration.Swagger);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();