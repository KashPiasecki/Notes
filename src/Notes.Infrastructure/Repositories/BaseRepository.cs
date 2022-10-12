using Notes.Application.Common.Interfaces;

namespace Notes.Infrastructure.Repositories;

public abstract class BaseRepository
{
    protected readonly IDataContext DataContext;

    protected BaseRepository(IDataContext dataContext)
    {
        DataContext = dataContext;
    }
}