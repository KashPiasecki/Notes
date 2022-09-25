using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS;

public abstract class BaseHandler<T>
{
    protected readonly IDataContext DataContext;
    protected readonly ILogger<T> Logger;

    protected BaseHandler(IDataContext dataContext, ILogger<T> logger)
    {
        DataContext = dataContext;
        Logger = logger;
    }
}