using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Note.Queries;

namespace Notes.Application.CQRS.Note.Commands.Create;

public record CreateNoteCommand(string Title, string Content) : IRequest<GetNoteDto>
{
    [JsonIgnore]
    public string? UserId { get; set; }
}

public sealed class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(255);
    }
}

public class CreateNoteCommandHandler : BaseEntityHandler<CreateNoteCommandHandler>, IRequestHandler<CreateNoteCommand, GetNoteDto>
{
    public CreateNoteCommandHandler(IDataContext dataContext, IMapper mapper, ILogger<CreateNoteCommandHandler> logger) : base(dataContext, mapper, logger)
    {
    }

    public async Task<GetNoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Request to create note");
        var note = Mapper.Map<Domain.Entities.Note>(request);
        var newNote = (await DataContext.Notes.AddAsync(note, cancellationToken)).Entity;
        await DataContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Successfully created note with id {NoteId}", newNote.Id);
        return Mapper.Map<GetNoteDto>(newNote);
    }
}