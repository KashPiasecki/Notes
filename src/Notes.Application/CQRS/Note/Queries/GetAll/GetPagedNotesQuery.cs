using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Filtering;
using Notes.Application.Pagination;
using Notes.Domain.Contracts;

namespace Notes.Application.CQRS.Note.Queries.GetAll;

public record GetPagedNotesQuery(string Route, PaginationFilter PaginationFilter, NoteFilter NoteFilter) : IRequest<PagedResponse<GetNoteDto>>;

public class GetAllNotesQueryHandler : BaseEntityHandler<GetAllNotesQueryHandler>, IRequestHandler<GetPagedNotesQuery, PagedResponse<GetNoteDto>>
{
    private readonly IPaginationHelper _paginationHelper;

    public GetAllNotesQueryHandler(IPaginationHelper paginationHelper, IDataContext dataContext, IMapper mapper,
        ILogger<GetAllNotesQueryHandler> logger) : base(dataContext, mapper, logger)
    {
        _paginationHelper = paginationHelper;
    }

    public async Task<PagedResponse<GetNoteDto>> Handle(GetPagedNotesQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to get all notes");
        var notes = await DataContext.Notes.Include(x => x.User)
            .Where(x => x.Title.ToLower().Contains(request.NoteFilter.Title.ToLower()))
            .Where(x => x.Content.ToLower().Contains(request.NoteFilter.Content.ToLower()))
            .Skip((request.PaginationFilter.PageNumber - 1) * request.PaginationFilter.PageSize)
            .Take(request.PaginationFilter.PageSize)
            .ToListAsync(cancellationToken);
        var notesDto = Mapper.Map<IEnumerable<GetNoteDto>>(notes);
        var totalRecords = await DataContext.Notes
            .AsNoTracking()
            .Where(x => x.Title.ToLower().Contains(request.NoteFilter.Title.ToLower()))
            .Where(x => x.Content.ToLower().Contains(request.NoteFilter.Content.ToLower()))
            .CountAsync(cancellationToken);
        var pagedResponse = _paginationHelper.CreatePagedReponse(notesDto, request.PaginationFilter, totalRecords, request.Route);
        Logger.LogInformation("Successfully retrieved all notes");
        return pagedResponse;
    }
}