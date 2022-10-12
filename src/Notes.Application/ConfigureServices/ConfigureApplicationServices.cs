using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Identity;

namespace Notes.Application.ConfigureServices;

public static class ConfigureApplicationServices
{
    public static void AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
        serviceCollection.AddHttpContextAccessor();
    }
}