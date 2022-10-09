using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Profiles;

namespace Notes.Application.ConfigureServices;

public static class ConfigureAutoMapper
{
    public static void AddAutoMapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(NotesProfile));
    }
}