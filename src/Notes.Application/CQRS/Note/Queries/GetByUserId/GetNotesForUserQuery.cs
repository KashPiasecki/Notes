using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Queries.GetByUserId;

public record GetNotesForUserQuery(string UserId) : IRequest<IEnumerable<GetNoteDto>>;

public class GetNotesByUserIdQueryHandler : BaseHandler<GetNotesByUserIdQueryHandler>, IRequestHandler<GetNotesForUserQuery, IEnumerable<GetNoteDto>>
{
    public GetNotesByUserIdQueryHandler(IDataContext dataContext, ILogger<GetNotesByUserIdQueryHandler> logger) : base(dataContext, logger)
    {
    }

    public async Task<IEnumerable<GetNoteDto>> Handle(GetNotesForUserQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request for notes for {UserId}", request.UserId);
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
        Logger.LogInformation("Successfully retrieved all notes for {UserId}", request.UserId);
        return notesDto;
    }
}