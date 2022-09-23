using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Commands.Delete;

public record DeleteNoteForUserCommand(Guid Id, string UserId) : IRequest;

public class DeleteNoteForUserCommandHandler : BaseHandler, IRequestHandler<DeleteNoteForUserCommand>
{
    public DeleteNoteForUserCommandHandler(IDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<Unit> Handle(DeleteNoteForUserCommand request, CancellationToken cancellationToken)
    {
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.UserId.Equals(request.UserId) && x.Id.Equals(request.Id));
        DataContext.Notes.Remove(note);
        await DataContext.SaveChangesAsync();
        return Unit.Value;
    }
}