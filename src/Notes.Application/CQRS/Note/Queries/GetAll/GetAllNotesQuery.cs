using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Queries.GetAll;

public record GetAllNotesQuery : IRequest<IEnumerable<GetNoteDto>>;

public class GetAllNotesQueryHandler : BaseHandler, IRequestHandler<GetAllNotesQuery, IEnumerable<GetNoteDto>>
{
    public GetAllNotesQueryHandler(IDataContext dataContext) : base(dataContext)
    {
    }
    
    public async Task<IEnumerable<GetNoteDto>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
    {
        var notes = await DataContext.Notes.ToListAsync(cancellationToken);
        var notesDto = notes.Select(x => new GetNoteDto
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            CreationDate = x.CreationDate,
            LastTimeModified = x.LastTimeModified
        });
        return notesDto;
    }
}