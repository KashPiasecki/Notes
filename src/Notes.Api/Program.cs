using Notes.Api.ConfigureServices;
using Notes.Api.Middlewares;
using Notes.Application.ConfigureServices;
using Notes.Infrastructure.ConfigureServices;

// WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);
builder.UseSerilog();

// ServiceCollection - Api
var serviceCollection = builder.Services;
var notesConfiguration = serviceCollection.AddConfigurations(builder.Configuration);
serviceCollection.AddControllers();
serviceCollection.AddTransient<ExceptionHandlingMiddleware>();

// ServiceCollection - Application
serviceCollection.AddApplication();
serviceCollection.AddCQRS();

// ServiceCollection - Infrastructure
serviceCollection.AddInfrastructureServices();
serviceCollection.AddRedis(notesConfiguration.Redis);
serviceCollection.AddPostgresDatabase(notesConfiguration.Database);
serviceCollection.AddIdentity(notesConfiguration.JwtSettings.Secret);
serviceCollection.AddAutoMapper();
serviceCollection.AddSwagger(notesConfiguration.Swagger);

// WebApplication
var app = builder.Build();
app.MigrateDatabase();
app.UseSwagger(notesConfiguration.Swagger);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();