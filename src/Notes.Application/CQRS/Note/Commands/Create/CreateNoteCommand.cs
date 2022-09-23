using MediatR;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Create;

public record CreateNoteCommand(string Title, string Content);

public record CreateNoteCommandWithUserId(string Title, string Content, string UserId) : CreateNoteCommand(Title, Content), IRequest<GetNoteDto>;

public class CreateNoteCommandHandler : BaseHandler, IRequestHandler<CreateNoteCommandWithUserId, GetNoteDto>
{
    public CreateNoteCommandHandler(IDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<GetNoteDto> Handle(CreateNoteCommandWithUserId request, CancellationToken cancellationToken)
    {
        var note = new Domain.Entities.Note
        {
            UserId = request.UserId,
            Title = request.Title,
            Content = request.Content,
            CreationDate = DateTime.UtcNow,
            LastTimeModified = DateTime.UtcNow
        };
        var newNote = await DataContext.Notes.AddAsync(note);
        await DataContext.SaveChangesAsync();
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