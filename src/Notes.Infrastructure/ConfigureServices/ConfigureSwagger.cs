using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Notes.Domain.Configurations;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureSwagger
{
    public static void AddSwagger(this IServiceCollection serviceCollection, SwaggerConfiguration swaggerConfiguration)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.SwaggerDoc($"{swaggerConfiguration.Version}", new OpenApiInfo
            {
                Title = swaggerConfiguration.Name,
                Version = swaggerConfiguration.Version,
                Contact = new OpenApiContact
                {
                    Name = swaggerConfiguration.Developer,
                    Email = swaggerConfiguration.Email
                }
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
        });
    }

    public static void UseSwagger(this WebApplication webApplication, SwaggerConfiguration swaggerConfiguration)
    {
        webApplication.UseSwagger(options => { options.RouteTemplate = "/documentation/{documentName}/swagger.json"; });
        webApplication.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/documentation/{swaggerConfiguration.Version}/swagger.json", $"{swaggerConfiguration.Name} {swaggerConfiguration.Version}");
            options.RoutePrefix = "documentation";
        });
    }
}