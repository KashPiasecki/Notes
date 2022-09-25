using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Queries.GetAll;

public record GetAllNotesQuery : IRequest<IEnumerable<GetNoteDto>>;

public class GetAllNotesQueryHandler : BaseHandler<GetAllNotesQueryHandler>, IRequestHandler<GetAllNotesQuery, IEnumerable<GetNoteDto>>
{
    public GetAllNotesQueryHandler(IDataContext dataContext, ILogger<GetAllNotesQueryHandler> logger) : base(dataContext, logger)
    {
    }

    public async Task<IEnumerable<GetNoteDto>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to get all notes");
        var notes = await DataContext.Notes.Include(x => x.User).ToListAsync(cancellationToken);
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
        Logger.LogInformation("Successfully retrieved all notes");
        return notesDto;
    }
}