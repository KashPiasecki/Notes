using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Filtering;
using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts.Responses;

namespace Notes.Application.CQRS.Note.Queries.GetByUserId;

public record GetPagedNotesForUserQuery
    (string UserId, PaginationFilterQuery PaginationFilterQuery, string Route, NoteFilterQuery NoteFilterQuery) : IRequest<PagedResponse<GetNoteDto>>;

public class GetPagedNotesForUserQueryHandlerWithMapping : BaseHandlerWithMapping<GetPagedNotesForUserQueryHandlerWithMapping>,
    IRequestHandler<GetPagedNotesForUserQuery, PagedResponse<GetNoteDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IPaginationHandler _paginationHandler;


    public GetPagedNotesForUserQueryHandlerWithMapping(IUnitOfWork unitOfWork, IMapper mapper, INoteRepository noteRepository, IPaginationHandler paginationHandler, ILogger<GetPagedNotesForUserQueryHandlerWithMapping> logger) : base(unitOfWork, mapper, logger)
    {
        _noteRepository = noteRepository;
        _paginationHandler = paginationHandler;
    }

    public async Task<PagedResponse<GetNoteDto>> Handle(GetPagedNotesForUserQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request for notes for {UserId}", request.UserId);
        var validPaginationFilter = _paginationHandler.ValidateQuery(request.PaginationFilterQuery);
        var notes = await _noteRepository.GetAllForUserAsync(request.UserId, validPaginationFilter, request.NoteFilterQuery, cancellationToken);
        var notesDto = Mapper.Map<IEnumerable<GetNoteDto>>(notes);
        Logger.LogInformation("Successfully retrieved notes for {UserId}", request.UserId);
        var totalRecords = await _noteRepository.CountForUserAsync(request.UserId, request.NoteFilterQuery, cancellationToken);
        var pagedResponse = _paginationHandler.CreatePagedResponse(notesDto, validPaginationFilter, totalRecords, request.Route);
        return pagedResponse;
    }
}