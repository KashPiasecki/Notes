using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Configurations;
using Notes.Domain.Contracts;
using Notes.Infrastructure.Persistence;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureDatabase
{
    public static void AddPostgresDatabase(this IServiceCollection service, DatabaseConfiguration database)
    {
        service.AddDbContext<DataContext>(options => options.UseNpgsql(database.ConnectionString));
        service.AddScoped<IDataContext, DataContext>();
    }
    
    public static async void MigrateDatabase(this WebApplication webApplication)
    {
        using var serviceScope = webApplication.Services.CreateScope();
        await using var dataContext = serviceScope.ServiceProvider.GetService<DataContext>();
        if (dataContext.Database.IsNpgsql())
        {
            await dataContext.Database.MigrateAsync();
        }

        await BootstrapRolesAsync(serviceScope);
    }

    private static async Task BootstrapRolesAsync(IServiceScope serviceScope)
    {
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(RoleNames.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleNames.Admin));
        }

        if (!await roleManager.RoleExistsAsync(RoleNames.User))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleNames.User));
        }
    }
}