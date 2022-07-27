using Microsoft.OpenApi.Models;

namespace Notes.Api.ProgramExtensions;

public static class SwaggerExtensions
{
    public static void AddSwaggerGenerator(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Notes Api",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Łukasz Piasecki",
                    Email = "kash.piasecki@protonmail.com"
                }
            });
        });
    }

    public static void UseSwagger(this WebApplication webApplication)
    {
        webApplication.UseSwagger(options => { options.RouteTemplate = "/documentation/{documentName}/swagger.json"; });
        webApplication.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/documentation/v1/swagger.json", "Notes Api v1");
            options.RoutePrefix = "documentation";
        });
    }
}