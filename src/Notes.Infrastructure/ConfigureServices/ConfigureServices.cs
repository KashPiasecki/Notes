using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Infrastructure.Identity;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureServices
{
    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITokenGenerator, TokenGenerator>();
    }
}