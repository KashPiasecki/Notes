using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Infrastructure.Pagination;
using Notes.Infrastructure.Repositories;
using Notes.Infrastructure.Utility.Wrappers;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IPaginationHelper, PaginationHelper>();
        serviceCollection.AddSingleton<IJsonConverterWrapper, JsonConverterWrapper>();
        serviceCollection.AddSingleton<IUriService>(o =>
        {
            var accessor = o.GetRequiredService<IHttpContextAccessor>();
            var request = accessor.HttpContext?.Request;
            var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
            return new UriService(uri);
        });
        serviceCollection.AddScoped<INoteRepository, NoteRepository>();
    }
}