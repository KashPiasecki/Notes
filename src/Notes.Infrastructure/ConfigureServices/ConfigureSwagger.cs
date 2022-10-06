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
            options.EnableAnnotations();
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
            options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer <JWT>`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
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