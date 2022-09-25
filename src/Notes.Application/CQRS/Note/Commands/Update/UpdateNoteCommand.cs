using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Update;

public record UpdateNoteCommand(Guid Id, string Title, string Content) : IRequest<GetNoteDto>;

public class UpdateNoteCommandHandler : BaseHandler<UpdateNoteCommandHandler>, IRequestHandler<UpdateNoteCommand, GetNoteDto>
{
    public UpdateNoteCommandHandler(IDataContext dataContext, ILogger<UpdateNoteCommandHandler> logger) : base(dataContext, logger)
    {
    }

    public async Task<GetNoteDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to update note {NoteId}", request.Id);
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NullReferenceException("Note with given id does not exist");
        }
        note.Title = request.Title;
        note.Content = request.Content;
        note.LastTimeModified = DateTime.UtcNow;
        await DataContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully updated note {NoteId}", request.Id);
        return new GetNoteDto
        {
            UserId = note.UserId,
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreationDate = note.CreationDate,
            LastTimeModified = note.LastTimeModified
            
        };
    }
}