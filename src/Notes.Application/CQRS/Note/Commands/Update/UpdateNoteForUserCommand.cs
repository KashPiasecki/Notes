using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Update;

public record UpdateNoteForUserCommand(Guid Id, string Title, string Content, string UserId) : IRequest<GetNoteDto>;

public class UpdateNoteForUserCommandHandler : BaseHandler, IRequestHandler<UpdateNoteForUserCommand, GetNoteDto>
{
    public UpdateNoteForUserCommandHandler(IDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<GetNoteDto> Handle(UpdateNoteForUserCommand request, CancellationToken cancellationToken)
    {
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.UserId.Equals(request.UserId) && x.Id == request.Id);
        note.Title = request.Title;
        note.Content = request.Content;
        note.LastTimeModified = DateTime.UtcNow;
        await DataContext.SaveChangesAsync();
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