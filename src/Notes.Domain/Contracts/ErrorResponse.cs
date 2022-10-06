namespace Notes.Domain.Contracts;

public class ErrorResponse
{
    public string Title { get; init; }
    public int StatusCode { get; init; }
    public string Details { get; init; }
    public IReadOnlyDictionary<string, string[]> Errors { get; init; }
}