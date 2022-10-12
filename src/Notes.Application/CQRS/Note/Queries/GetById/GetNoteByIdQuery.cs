using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Queries.GetById;

public record GetNoteByIdQuery(Guid Id) : IRequest<GetNoteDto>;

public class GetNoteByIdQueryHandler : BaseEntityHandler<GetNoteByIdQueryHandler>, IRequestHandler<GetNoteByIdQuery, GetNoteDto>
{
    public GetNoteByIdQueryHandler(IDataContext dataContext, IMapper mapper, ILogger<GetNoteByIdQueryHandler> logger) : base(dataContext, mapper, logger)
    {
    }

    public async Task<GetNoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to get note with id: {NoteId}", request.Id);
        var note = await DataContext.Notes.Include(x => x.User)
            .SingleOrDefaultAsync(note => note.Id == request.Id, cancellationToken: cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NotFoundException("Note with given id does not exist");
        }

        Logger.LogInformation("Successfully retrieved note with id: {NoteId}", request.Id);
        return Mapper.Map<GetNoteDto>(note);
    }
}