using Microsoft.Extensions.DependencyInjection;

namespace Notes.Application.ConfigureServices;

public static class ConfigureApplicationServices
{
    public static void AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpContextAccessor();
    }
}