using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Create;

public record CreateNoteCommand(string Title, string Content);

public record CreateNoteCommandWithUserId(string Title, string Content, string UserId) : CreateNoteCommand(Title, Content), IRequest<GetNoteDto>;

public class CreateNoteCommandHandler : BaseHandler<CreateNoteCommandHandler>, IRequestHandler<CreateNoteCommandWithUserId, GetNoteDto>
{
    public CreateNoteCommandHandler(IDataContext dataContext, ILogger<CreateNoteCommandHandler> logger) : base(dataContext, logger)
    {
    }

    public async Task<GetNoteDto> Handle(CreateNoteCommandWithUserId request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to create note");
        var note = new Domain.Entities.Note
        {
            UserId = request.UserId,
            Title = request.Title,
            Content = request.Content,
            CreationDate = DateTime.UtcNow,
            LastTimeModified = DateTime.UtcNow
        };
        var newNote = await DataContext.Notes.AddAsync(note, cancellationToken);
        await DataContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully created note with id {NoteId}", newNote.Entity.Id);
        return new GetNoteDto
        {
            Id = newNote.Entity.Id,
            UserId = newNote.Entity.UserId,
            Title = newNote.Entity.Title,
            Content = newNote.Entity.Content,
            CreationDate = newNote.Entity.CreationDate,
            LastTimeModified = newNote.Entity.LastTimeModified
        };
    }
}