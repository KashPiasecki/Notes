using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Commands.Delete;

public record DeleteNoteCommand(Guid Id) : IRequest;

public class DeleteNoteCommandHandler : BaseHandler, IRequestHandler<DeleteNoteCommand>
{
    public DeleteNoteCommandHandler(IDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.Id == request.Id);
        DataContext.Notes.Remove(note);
        await DataContext.SaveChangesAsync();
        return Unit.Value;
    }
}