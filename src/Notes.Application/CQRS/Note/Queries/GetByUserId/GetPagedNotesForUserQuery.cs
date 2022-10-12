using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Filtering;
using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts;

namespace Notes.Application.CQRS.Note.Queries.GetByUserId;

public record GetPagedNotesForUserQuery
    (string UserId, PaginationFilterQuery PaginationFilterQuery, string Route, NoteFilterQuery NoteFilterQuery) : IRequest<PagedResponse<GetNoteDto>>;

public class GetPagedNotesForUserQueryHandler : BaseEntityHandler<GetPagedNotesForUserQueryHandler>,
    IRequestHandler<GetPagedNotesForUserQuery, PagedResponse<GetNoteDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IPaginationHelper _paginationHelper;

    public GetPagedNotesForUserQueryHandler(INoteRepository noteRepository, IPaginationHelper paginationHelper, IDataContext dataContext,
        IMapper mapper, ILogger<GetPagedNotesForUserQueryHandler> logger) : base(
        dataContext, mapper, logger)
    {
        _paginationHelper = paginationHelper;
        _noteRepository = noteRepository;
    }

    public async Task<PagedResponse<GetNoteDto>> Handle(GetPagedNotesForUserQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request for notes for {UserId}", request.UserId);
        var validPaginationFilter = _paginationHelper.ValidateQuery(request.PaginationFilterQuery);
        var notes = await _noteRepository.GetNotesAsync(validPaginationFilter, request.NoteFilterQuery, cancellationToken);
        var notesDto = Mapper.Map<IEnumerable<GetNoteDto>>(notes);
        Logger.LogInformation("Successfully retrieved notes for {UserId}", request.UserId);
        var totalRecords = await _noteRepository.CountAsync(request.NoteFilterQuery, cancellationToken);
        var pagedResponse = _paginationHelper.CreatePagedResponse(notesDto, validPaginationFilter, totalRecords, request.Route);
        return pagedResponse;
    }
}