using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Filtering;
using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts.Responses;

namespace Notes.Application.CQRS.Note.Queries.GetAll;

public record GetPagedNotesQuery
    (string Route, PaginationFilterQuery PaginationFilterQuery, NoteFilterQuery NoteFilterQuery) : IRequest<PagedResponse<GetNoteDto>>;

public class GetAllNotesQueryHandler : BaseHandlerWithMapping<GetAllNotesQueryHandler>, IRequestHandler<GetPagedNotesQuery, PagedResponse<GetNoteDto>>
{
    private readonly IPaginationHandler _paginationHandler;

    public GetAllNotesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPaginationHandler paginationHandler,
        ILogger<GetAllNotesQueryHandler> logger) : base(unitOfWork, mapper, logger)
    {
        _paginationHandler = paginationHandler;
    }

    public async Task<PagedResponse<GetNoteDto>> Handle(GetPagedNotesQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to get all notes");
        var paginationFilter = _paginationHandler.ValidateQuery(request.PaginationFilterQuery);
        var notes = await UnitOfWork.Notes.GetAllAsync(paginationFilter, request.NoteFilterQuery, cancellationToken);
        var notesDto = Mapper.Map<IEnumerable<GetNoteDto>>(notes);
        var totalRecords = await UnitOfWork.Notes.CountAsync(request.NoteFilterQuery, cancellationToken);
        var pagedResponse = _paginationHandler.CreatePagedResponse(notesDto, paginationFilter, totalRecords, request.Route);
        Logger.LogInformation("Successfully retrieved all notes");
        return pagedResponse;
    }
}