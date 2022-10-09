using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;
using Notes.Domain.Contracts.Exceptions;

namespace Notes.Application.CQRS.Note.Commands.Update;

public record UpdateNoteCommand(Guid Id, string Title, string Content) : IRequest<GetNoteDto>;

public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(255);
    }
}

public class UpdateNoteCommandHandler : BaseEntityHandler<UpdateNoteCommandHandler>, IRequestHandler<UpdateNoteCommand, GetNoteDto>
{
    public UpdateNoteCommandHandler(IDataContext dataContext, IMapper mapper, ILogger<UpdateNoteCommandHandler> logger) : base(dataContext, mapper, logger)
    {
    }

    public async Task<GetNoteDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to update note {NoteId}", request.Id);
        var note = await DataContext.Notes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NotFoundException("Note with given id does not exist");
        }
        
        Mapper.Map(request, note);
        await DataContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully updated note {NoteId}", request.Id);
        return Mapper.Map<GetNoteDto>(note);
    }
}