using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Infrastructure.Utility.Wrappers;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IJsonConverterWrapper, JsonConverterWrapper>();
    }
}