using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Infrastructure.Repositories;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureRepositories
{
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<INoteRepository, NoteRepository>();
        serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }
}