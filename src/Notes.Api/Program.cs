using Notes.Api.ProgramExtensions;

// WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// ServiceCollection
var services = builder.Services;
services.AddControllers();
services.AddSwaggerGenerator();

// WebApplication
var app = builder.Build();
app.UseSwagger();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();