using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Filtering;
using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts;

namespace Notes.Application.CQRS.Note.Queries.GetAll;

public record GetPagedNotesQuery(string Route, PaginationFilterQuery PaginationFilterQuery, NoteFilterQuery NoteFilterQuery) : IRequest<PagedResponse<GetNoteDto>>;

public class GetAllNotesQueryHandler : BaseEntityHandler<GetAllNotesQueryHandler>, IRequestHandler<GetPagedNotesQuery, PagedResponse<GetNoteDto>>
{
    private readonly IPaginationHelper _paginationHelper;
    private readonly INoteRepository _notesRepository;
    

    public GetAllNotesQueryHandler(INoteRepository notesRepository
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        , IPaginationHelper paginationHelper, IDataContext dataContext,
        IMapper mapper, ILogger<GetAllNotesQueryHandler> logger) : base(dataContext, mapper, logger)
    {
        _paginationHelper = paginationHelper;
        _notesRepository = notesRepository;
    }

    public async Task<PagedResponse<GetNoteDto>> Handle(GetPagedNotesQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to get all notes");
        var paginationFilter = _paginationHelper.ValidateQuery(request.PaginationFilterQuery);
        var notes = await _notesRepository.GetNotesAsync(paginationFilter, request.NoteFilterQuery, cancellationToken);
        var notesDto = Mapper.Map<IEnumerable<GetNoteDto>>(notes);
        var totalRecords = await _notesRepository.CountAsync(request.NoteFilterQuery, cancellationToken);
        var pagedResponse = _paginationHelper.CreatePagedResponse(notesDto, paginationFilter, totalRecords, request.Route);
        Logger.LogInformation("Successfully retrieved all notes");
        return pagedResponse;
    }
}