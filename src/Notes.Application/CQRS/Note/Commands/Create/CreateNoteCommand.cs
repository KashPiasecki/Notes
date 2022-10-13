using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Create;

public record CreateNoteCommand(string Title, string Content) : IRequest<GetNoteDto>
{
    [JsonIgnore] public string? UserId { get; set; }
}

public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(255);
    }
}

public class CreateNoteCommandHandlerWithMapping : BaseHandlerWithMapping<CreateNoteCommandHandlerWithMapping>, IRequestHandler<CreateNoteCommand, GetNoteDto>
{
    public CreateNoteCommandHandlerWithMapping(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateNoteCommandHandlerWithMapping> logger) : base(unitOfWork, mapper,
        logger)
    {
    }

    public async Task<GetNoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to create note");
        var note = Mapper.Map<Domain.Entities.Note>(request);
        var newNote = await UnitOfWork.Notes.AddAsync(note, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully created note with id {NoteId}", newNote.Id);
        var byIdAsync = await UnitOfWork.Notes.GetByIdAsync(newNote.Id, cancellationToken);
        return Mapper.Map<GetNoteDto>(newNote);
    }
}