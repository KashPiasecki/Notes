using Notes.Domain.Entities;

namespace Notes.Domain.Contracts.Responses;

public class TokenResponse
{
    public string Token { get; init; }
    public RefreshToken RefreshToken { get; init; }
    
}