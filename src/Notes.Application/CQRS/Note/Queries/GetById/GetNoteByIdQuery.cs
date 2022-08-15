using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Queries.GetById;

public record GetNoteByIdQuery(Guid Id) : IRequest<GetNoteDto>;

public class GetNoteByIdQueryHandler : BaseHandler, IRequestHandler<GetNoteByIdQuery, GetNoteDto>
{
    public GetNoteByIdQueryHandler(IDataContext dataContenotet) : base(dataContenotet)
    {
    }
    
    public async Task<GetNoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var note = await DataContext.Notes.SingleOrDefaultAsync(note => note.Id == request.Id);
        if (note is not null)
        {
            return  new GetNoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreationDate = note.CreationDate,
                LastTimeModified = note.LastTimeModified
            };    
        }

        throw new NullReferenceException("No note was found");
    }
}