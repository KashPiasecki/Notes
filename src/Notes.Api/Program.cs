// WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// ServiceCollection
var services = builder.Services;
services.AddControllers();
services.AddSwaggerGen();

// WebApplication
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();