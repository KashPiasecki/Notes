namespace Notes.Domain.Contracts.Exceptions;

public abstract class ApplicationException : Exception
{
    public string Title { get; init; }

    protected ApplicationException(string title, string message) : base(message)
    {
        Title = title;
    }
}