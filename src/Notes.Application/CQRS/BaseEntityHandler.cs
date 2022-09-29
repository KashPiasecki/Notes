using AutoMapper;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS;

public abstract class BaseEntityHandler<T> : BaseHandler<T>
{
    protected readonly IMapper Mapper;

    protected BaseEntityHandler(IDataContext dataContext, IMapper mapper, ILogger<T> logger) : base(dataContext, logger)
    {
        Mapper = mapper;
    }
}