namespace Notes.Domain.Identity;

public class AuthenticationFailedResult : AuthenticationResult
{
    public IEnumerable<string> Errors { get; set; }
}