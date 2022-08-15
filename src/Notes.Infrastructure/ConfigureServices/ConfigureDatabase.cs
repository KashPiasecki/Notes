using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Infrastructure.Configuration;
using Notes.Infrastructure.Persistence;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureDatabase
{
    public static void AddPostgresDatabase(this IServiceCollection service, DatabaseConfiguration database)
    {
        service.AddDbContext<DataContext>(options => options.UseNpgsql(database.ConnectionString));
        service.AddScoped<IDataContext, DataContext>();
    }
    
    public static void MigrateDatabase(this WebApplication webApplication)
    {
        using var serviceScope = webApplication.Services.CreateScope();
        using var dataContext = serviceScope.ServiceProvider.GetService<DataContext>();
        dataContext?.Database.Migrate();
    }
}