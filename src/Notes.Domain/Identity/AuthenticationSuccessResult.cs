namespace Notes.Domain.Identity;

public class AuthenticationSuccessResult : AuthenticationResult
{
    public string Token { get; init; }
}