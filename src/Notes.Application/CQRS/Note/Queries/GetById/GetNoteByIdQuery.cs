using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Queries.GetById;

public record GetNoteByIdQuery(Guid Id) : IRequest<GetNoteDto>;

public class GetNoteByIdQueryHandler : BaseHandler<GetNoteByIdQueryHandler>, IRequestHandler<GetNoteByIdQuery, GetNoteDto>
{
    public GetNoteByIdQueryHandler(IDataContext dataContext, ILogger<GetNoteByIdQueryHandler> logger) : base(dataContext, logger)
    {
    }

    public async Task<GetNoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to get note with id: {NoteId}", request.Id);
        var note = await DataContext.Notes.Include(x => x.User)
            .SingleOrDefaultAsync(note => note.Id == request.Id, cancellationToken: cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NullReferenceException("Note with given id does not exist");
        }

        Logger.LogInformation("Successfully retrieved note with id: {NoteId}", request.Id);
        return new GetNoteDto
        {
            Id = note.Id,
            UserName = note.User.UserName,
            UserId = note.User.Id,
            Title = note.Title,
            Content = note.Content,
            CreationDate = note.CreationDate,
            LastTimeModified = note.LastTimeModified
        };
    }
}