using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Identity;

namespace Notes.Application.ConfigureServices;

public static class ConfigureServices
{
    public static void AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
    }
}