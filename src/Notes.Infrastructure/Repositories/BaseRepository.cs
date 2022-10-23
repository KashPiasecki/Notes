using Notes.Infrastructure.Persistence;

namespace Notes.Infrastructure.Repositories;

public abstract class BaseRepository
{
    protected readonly DataContext DataContext;

    protected BaseRepository(DataContext dataContext)
    {
        DataContext = dataContext;
    }
}