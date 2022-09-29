using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Commands.Delete;

public record DeleteNoteForUserCommand(Guid Id, string UserId) : IRequest;

public class DeleteNoteForUserCommandHandler : BaseEntityHandler<DeleteNoteForUserCommandHandler>, IRequestHandler<DeleteNoteForUserCommand>
{
    public DeleteNoteForUserCommandHandler(IDataContext dataContext, IMapper mapper, ILogger<DeleteNoteForUserCommandHandler> logger) : base(dataContext, mapper, logger)
    {
    }

    public async Task<Unit> Handle(DeleteNoteForUserCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to delete note {NoteId} for user {UserId}", request.Id, request.UserId);
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.UserId.Equals(request.UserId) && x.Id.Equals(request.Id),
            cancellationToken: cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NullReferenceException("Note with given id does not exist");
        }
        
        DataContext.Notes.Remove(note);
        await DataContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully deleted note {NoteId} for user {UserId}", request.Id, request.UserId);
        return Unit.Value;
    }
}