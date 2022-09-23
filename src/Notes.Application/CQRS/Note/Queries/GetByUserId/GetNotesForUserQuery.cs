using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Queries.GetByUserId;

public record GetNotesForUserQuery(string UserId) : IRequest<IEnumerable<GetNoteDto>>;

public class GetNotesByUserIdQueryHandler : BaseHandler, IRequestHandler<GetNotesForUserQuery, IEnumerable<GetNoteDto>>
{
    public GetNotesByUserIdQueryHandler(IDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<IEnumerable<GetNoteDto>> Handle(GetNotesForUserQuery request, CancellationToken cancellationToken)
    {
        var notes = await DataContext.Notes.Where(x => x.UserId == request.UserId).Include(x => x.User).ToListAsync(cancellationToken);
        var notesDto = notes.Select(x => new GetNoteDto
        {
            Id = x.Id,
            UserName = x.User.UserName,
            UserId = x.User.Id,
            Title = x.Title,
            Content = x.Content,
            CreationDate = x.CreationDate,
            LastTimeModified = x.LastTimeModified
        });
        return notesDto;
    }
}