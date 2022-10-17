using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Update;

public record UpdateNoteCommand(Guid Id, string Title, string Content) : IRequest<GetNoteDto>;

public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(255);
    }
}

public class UpdateNoteCommandHandler : BaseHandlerWithMapping<UpdateNoteCommandHandler>, IRequestHandler<UpdateNoteCommand, GetNoteDto>
{
    public UpdateNoteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateNoteCommandHandler> logger) : base(unitOfWork, mapper, logger)
    {
    }

    public async Task<GetNoteDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to update note {NoteId}", request.Id);
        var note = await UnitOfWork.Notes.GetByIdAsync(request.Id, cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}", request.Id);
            throw new NotFoundException("Note with given id does not exist");
        }
        
        Mapper.Map(request, note);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully updated note {NoteId}", request.Id);
        return Mapper.Map<GetNoteDto>(note);
    }
}