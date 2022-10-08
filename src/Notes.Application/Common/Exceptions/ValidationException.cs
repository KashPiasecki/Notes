namespace Notes.Application.Common.Exceptions;

public class ValidationException : ApplicationException
{
    public IReadOnlyDictionary<string, string[]> ErrorsDictionary { init; get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
        : base("Validation Failure", "One or more validation errors occurred")
        => ErrorsDictionary = errorsDictionary;
}