using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Update;

public record UpdateNoteForUserCommand(Guid Id, string Title, string Content) : IRequest<GetNoteDto>
{
    [JsonIgnore] public string? UserId { get; set; }
}

public class UpdateNoteForUserCommandValidator : AbstractValidator<UpdateNoteForUserCommand>
{
    public UpdateNoteForUserCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(255);
    }
}

public class UpdateNoteForUserCommandHandler : BaseHandlerWithMapping<UpdateNoteForUserCommandHandler>,
    IRequestHandler<UpdateNoteForUserCommand, GetNoteDto>
{
    public UpdateNoteForUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateNoteForUserCommandHandler> logger) : base(unitOfWork,
        mapper, logger)
    {
    }

    public async Task<GetNoteDto> Handle(UpdateNoteForUserCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to update note {NoteId} for user {UserId}", request.Id, request.UserId);
        var note = await UnitOfWork.Notes.GetByIdForUserAsync(request.UserId!, request.Id, cancellationToken);
        if (note is null)
        {
            Logger.LogError("Failed to get note with id: {NoteId}, it either doesn't exist or doesn't belong to user {UserId}", request.Id,
                request.UserId);
            throw new NotFoundException("Note with given id does not exist");
        }

        Mapper.Map(request, note);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully updated note {NoteId} for user {UserId}", request.Id, request.UserId);
        return Mapper.Map<GetNoteDto>(note);
    }
}