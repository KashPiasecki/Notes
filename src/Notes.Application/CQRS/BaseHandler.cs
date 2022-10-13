using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Repositories;

namespace Notes.Application.CQRS;

public abstract class BaseHandler<T>
{
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly ILogger<T> Logger;

    protected  BaseHandler(IUnitOfWork unitOfWork, ILogger<T> logger)
    {
        Logger = logger;
        UnitOfWork = unitOfWork;
    }
}