using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Update;

public record UpdateNoteForUserCommand(Guid Id, string Title, string Content, string UserId) : IRequest<GetNoteDto>;

public class UpdateNoteForUserCommandHandler : BaseEntityHandler<UpdateNoteForUserCommandHandler>, IRequestHandler<UpdateNoteForUserCommand, GetNoteDto>
{
    public UpdateNoteForUserCommandHandler(IDataContext dataContext, IMapper mapper, ILogger<UpdateNoteForUserCommandHandler> logger) : base(dataContext, mapper, logger)
    {
    }

    public async Task<GetNoteDto> Handle(UpdateNoteForUserCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to update note {NoteId} for user {UserId}", request.Id, request.UserId);
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.UserId.Equals(request.UserId) && x.Id == request.Id,
            cancellationToken: cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NullReferenceException("Note with given id does not exist");
        }

        Mapper.Map(request, note);
        await DataContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully updated note {NoteId} for user {UserId}", request.Id, request.UserId);
        return Mapper.Map<GetNoteDto>(note);
    }
}