using Notes.Domain.Identity;

namespace Notes.Application.Identity;

public class TokenResponse
{
    public string Token { get; init; }
    public RefreshToken RefreshToken { get; init; }
    
}