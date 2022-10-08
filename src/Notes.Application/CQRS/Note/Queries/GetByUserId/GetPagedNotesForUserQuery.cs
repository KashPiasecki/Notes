using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Pagination;
using Notes.Domain.Contracts;

namespace Notes.Application.CQRS.Note.Queries.GetByUserId;

public record GetPagedNotesForUserQuery(string UserId, PaginationFilter PaginationFilter, string Route) : IRequest<PagedResponse<GetNoteDto>>;

public class GetPagedNotesForUserQueryHandler : BaseEntityHandler<GetPagedNotesForUserQueryHandler>,
    IRequestHandler<GetPagedNotesForUserQuery, PagedResponse<GetNoteDto>>
{
    private readonly IPaginationHelper _paginationHelper;
    
    public GetPagedNotesForUserQueryHandler(IPaginationHelper paginationHelper, IDataContext dataContext, IMapper mapper, ILogger<GetPagedNotesForUserQueryHandler> logger) : base(
        dataContext, mapper, logger)
    {
        _paginationHelper = paginationHelper;
    }

    public async Task<PagedResponse<GetNoteDto>> Handle(GetPagedNotesForUserQuery request, CancellationToken cancellationToken)
    { 
        Logger.LogInformation("Request for notes for {UserId}", request.UserId);
        var pagedData = await DataContext.Notes
            .Where(x => x.UserId == request.UserId).Include(x => x.User)
            .Skip((request.PaginationFilter.PageNumber - 1) * request.PaginationFilter.PageSize)
            .Take(request.PaginationFilter.PageSize)
            .ToListAsync(cancellationToken);
        var notesDto = Mapper.Map<IEnumerable<GetNoteDto>>(pagedData);
        Logger.LogInformation("Successfully retrieved notes for {UserId}", request.UserId);
        var totalRecords = await DataContext.Notes
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .CountAsync(cancellationToken);
        var pagedResponse = _paginationHelper.CreatePagedReponse(notesDto, request.PaginationFilter, totalRecords, request.Route);
        return pagedResponse;
    }
}