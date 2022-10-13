using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Infrastructure.Jwt;
using Notes.Infrastructure.Pagination;
using Notes.Infrastructure.Utility.Wrappers;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
        serviceCollection.AddScoped<IPaginationHandler, PaginationHandler>();
        serviceCollection.AddSingleton<IJsonConverterWrapper, JsonConverterWrapper>();
        serviceCollection.AddSingleton<IUriService>(serviceProvider =>
        {
            var accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var request = accessor.HttpContext?.Request;
            var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
            return new UriService(uri);
        });
    }
}