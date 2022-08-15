using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Update;

public record UpdateNoteCommand(Guid Id, string Title, string Content) : IRequest<GetNoteDto>;

public class UpdateNoteCommandHandler : BaseHandler, IRequestHandler<UpdateNoteCommand, GetNoteDto>
{
    public UpdateNoteCommandHandler(IDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<GetNoteDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.Id == request.Id);
        note.Title = request.Title;
        note.Content = request.Content;
        note.LastTimeModified = DateTime.UtcNow;
        await DataContext.SaveChangesAsync();
        return new GetNoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreationDate = note.CreationDate,
            LastTimeModified = note.LastTimeModified
            
        };
    }
}