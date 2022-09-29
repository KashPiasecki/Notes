using Notes.Api.ConfigureServices;
using Notes.Application.ConfigureServices;
using Notes.Infrastructure.ConfigureServices;

// WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);
builder.UseSerilog();

// ServiceCollection - Api
var services = builder.Services;
var notesConfiguration = services.AddConfigurations(builder.Configuration);
services.AddControllers();

// ServiceCollection - Application
services.AddApplication();
services.AddMediatR();

// ServiceCollection - Infrastructure
services.AddPostgresDatabase(notesConfiguration.Database);
services.AddIdentity(notesConfiguration.JwtSettings.Secret);
services.AddAutoMapper();
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