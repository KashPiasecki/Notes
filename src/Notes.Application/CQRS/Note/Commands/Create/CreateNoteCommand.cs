using MediatR;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Create;

public record CreateNoteCommand(string Title, string Content) : IRequest<GetNoteDto>;

public class CreateNoteCommandHandler : BaseHandler, IRequestHandler<CreateNoteCommand, GetNoteDto>
{
    public CreateNoteCommandHandler(IDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<GetNoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Domain.Entities.Note
        {
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
            Title = newNote.Entity.Title,
            Content = newNote.Entity.Content,
            CreationDate = newNote.Entity.CreationDate,
            LastTimeModified = newNote.Entity.LastTimeModified
            
        };
    }
}