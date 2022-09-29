using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;

namespace Notes.Application.CQRS.Note.Queries.GetAll;

public record GetAllNotesQuery : IRequest<IEnumerable<GetNoteDto>>;

public class GetAllNotesQueryHandler : BaseEntityHandler<GetAllNotesQueryHandler>, IRequestHandler<GetAllNotesQuery, IEnumerable<GetNoteDto>>
{
    public GetAllNotesQueryHandler(IDataContext dataContext, IMapper mapper, ILogger<GetAllNotesQueryHandler> logger) : base(dataContext, mapper, logger)
    {
    }

    public async Task<IEnumerable<GetNoteDto>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to get all notes");
        var notes = await DataContext.Notes.Include(x => x.User).ToListAsync(cancellationToken);
        var notesDto = Mapper.Map<IEnumerable<GetNoteDto>>(notes);
        Logger.LogInformation("Successfully retrieved all notes");
        return notesDto;
    }
}