namespace Notes.Domain.Contracts.Identity;

public class AuthenticationSuccessResult : AuthenticationResult
{
    public string Token { get; init; }
    public string RefreshToken { get; init; }
    
}