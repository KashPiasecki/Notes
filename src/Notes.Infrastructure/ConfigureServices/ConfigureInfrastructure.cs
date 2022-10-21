using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Providers;
using Notes.Application.Common.Interfaces.Wrappers;
using Notes.Infrastructure.Jwt;
using Notes.Infrastructure.Pagination;
using Notes.Infrastructure.Utility.Providers;
using Notes.Infrastructure.Utility.Wrappers;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IContextInfoProvider, ContextInfoProvider>();
        serviceCollection.AddTransient<IJsonConverterWrapper, JsonConverterWrapper>();
        serviceCollection.AddTransient<IClaimsPrincipalInfoProvider, ClaimsPrincipalInfoProvider>();
        serviceCollection.AddTransient<IDistributedCacheWrapper, DistributedCacheWrapper>();
        serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
        serviceCollection.AddScoped<IPaginationHandler, PaginationHandler>();
        serviceCollection.AddScoped<IUserManagerWrapper, UserManagerWrapper>();
        serviceCollection.AddSingleton<IUriService>(serviceProvider =>
        {
            var accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var request = accessor.HttpContext?.Request;
            var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
            return new UriService(uri);
        });
    }
}