using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Commands.Delete;

public record DeleteNoteCommand(Guid Id) : IRequest;

public class DeleteNoteCommandHandler : BaseHandler<DeleteNoteCommandHandler>, IRequestHandler<DeleteNoteCommand>
{
    public DeleteNoteCommandHandler(IDataContext dataContext, ILogger<DeleteNoteCommandHandler> logger) : base(dataContext, logger)
    {
    }

    public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to delete note {NoteId}", request.Id);
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NullReferenceException("Note with given id does not exist");
        }
        DataContext.Notes.Remove(note);
        await DataContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully deleted note {NoteId}", request.Id);
        return Unit.Value;
    }
}