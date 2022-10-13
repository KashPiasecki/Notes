using Notes.Application.Common.Interfaces.Repositories;
using Notes.Infrastructure.Persistence;

namespace Notes.Infrastructure.Repositories;

public abstract class BaseRepository : IBaseRepository
{
    protected readonly DataContext DataContext;

    protected BaseRepository(DataContext dataContext)
    {
        DataContext = dataContext;
    }

    public async Task SaveChangesAsync()
    {
        await DataContext.SaveChangesAsync();
    }
}