namespace Notes.Domain.Contracts.Identity;

public class AuthenticationFailedResult : AuthenticationResult
{
    public IEnumerable<string> Errors { get; set; }
}