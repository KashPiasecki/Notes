using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;

namespace Notes.Application.CQRS.Note.Queries.GetById;

public record GetNoteByIdQuery(Guid Id) : IRequest<GetNoteDto>;

public class GetNoteByIdQueryHandler : BaseHandlerWithMapping<GetNoteByIdQueryHandler>, IRequestHandler<GetNoteByIdQuery, GetNoteDto>
{
    public GetNoteByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetNoteByIdQueryHandler> logger) : base(unitOfWork, mapper, logger)
    {
    }

    public async Task<GetNoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to get note with id: {NoteId}", request.Id);
        var note = await UnitOfWork.Notes.GetByIdAsync(request.Id, cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NotFoundException("Note with given id does not exist");
        }

        Logger.LogInformation("Successfully retrieved note with id: {NoteId}", request.Id);
        return Mapper.Map<GetNoteDto>(note);
    }
}