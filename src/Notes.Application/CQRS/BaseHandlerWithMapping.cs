using AutoMapper;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Repositories;

namespace Notes.Application.CQRS;

public abstract class BaseHandlerWithMapping<T> : BaseHandler<T>
{
    protected readonly IMapper Mapper;

    protected BaseHandlerWithMapping(IUnitOfWork unitOfWork, IMapper mapper, ILogger<T> logger) : base(unitOfWork, logger)
    {
        Mapper = mapper;
    }
}