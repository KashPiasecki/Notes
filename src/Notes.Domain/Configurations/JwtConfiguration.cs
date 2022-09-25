namespace Notes.Domain.Configurations;

public class JwtConfiguration
{
    public string Secret { get; init; }
    public string TokenLifetime { get; init; }
}