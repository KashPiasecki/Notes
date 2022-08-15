namespace Notes.Application.CQRS.Note.Queries;

public record GetNoteDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Content { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime LastTimeModified { get; init; }
}