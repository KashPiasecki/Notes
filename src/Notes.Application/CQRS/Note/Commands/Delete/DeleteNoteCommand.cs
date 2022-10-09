using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Contracts.Exceptions;

namespace Notes.Application.CQRS.Note.Commands.Delete;

public record DeleteNoteCommand(Guid Id) : IRequest;

public class DeleteNoteCommandHandler : BaseEntityHandler<DeleteNoteCommandHandler>, IRequestHandler<DeleteNoteCommand>
{
    public DeleteNoteCommandHandler(IDataContext dataContext, IMapper mapper, ILogger<DeleteNoteCommandHandler> logger) : base(dataContext, mapper, logger)
    {
    }

    public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to delete note {NoteId}", request.Id);
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NotFoundException("Note with given id does not exist");
        }
        
        DataContext.Notes.Remove(note);
        await DataContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully deleted note {NoteId}", request.Id);
        return Unit.Value;
    }
}