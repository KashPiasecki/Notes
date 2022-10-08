using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Identity;
using Notes.Application.Pagination;

namespace Notes.Application.ConfigureServices;

public static class ConfigureApplicationServices
{
    public static void AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddScoped<IPaginationHelper, PaginationHelper>();
        serviceCollection.AddSingleton<IUriService>(o =>
        {
            var accessor = o.GetRequiredService<IHttpContextAccessor>();
            var request = accessor.HttpContext?.Request;
            var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
            return new UriService(uri);
        });
    }
}