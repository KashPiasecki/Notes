using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;

namespace Notes.Application.CQRS.Note.Commands.Delete;

public record DeleteNoteForUserCommand(Guid Id) : IRequest
{
    [JsonIgnore]
    public string? UserId { get; set; }
    
}

public class DeleteNoteForUserCommandHandler : BaseHandler<DeleteNoteForUserCommandHandler>,
    IRequestHandler<DeleteNoteForUserCommand>
{
    public DeleteNoteForUserCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteNoteForUserCommandHandler> logger) :
        base(unitOfWork, logger)
    {
    }

    public async Task<Unit> Handle(DeleteNoteForUserCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to delete note {NoteId} for user {UserId}", request.Id, request.UserId);
        var note = await UnitOfWork.Notes.GetByIdForUserAsync(request.UserId!, request.Id, cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}, it either doesn't exist or doesn't belong to user {UserId}", request.Id,
                request.UserId);
            throw new NotFoundException("Note with given id does not exist");
        }

        UnitOfWork.Notes.Remove(note);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully deleted note {NoteId} for user {UserId}", request.Id, request.UserId);
        return Unit.Value;
    }
}