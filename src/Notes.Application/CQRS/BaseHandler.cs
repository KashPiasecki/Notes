using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS;

public class BaseHandler
{
    protected readonly IDataContext DataContext;

    protected BaseHandler(IDataContext dataContext)
    {
        DataContext = dataContext;
    }
}