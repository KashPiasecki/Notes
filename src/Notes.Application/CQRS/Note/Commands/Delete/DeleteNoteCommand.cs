using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;

namespace Notes.Application.CQRS.Note.Commands.Delete;

public record DeleteNoteCommand(Guid Id) : IRequest;

public class DeleteNoteCommandHandlerWithMapping : BaseHandler<DeleteNoteCommandHandlerWithMapping>, IRequestHandler<DeleteNoteCommand>
{
    public DeleteNoteCommandHandlerWithMapping(IUnitOfWork unitOfWork, ILogger<DeleteNoteCommandHandlerWithMapping> logger) : base(unitOfWork, logger)
    {
    }

    public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to delete note {NoteId}", request.Id);
        var note = await UnitOfWork.Notes.GetNoteByIdAsync(request.Id, cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NotFoundException("Note with given id does not exist");
        }

        UnitOfWork.Notes.Remove(note);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully deleted note {NoteId}", request.Id);
        return Unit.Value;
    }
}