using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Configurations;
using Notes.Domain.Contracts;
using Notes.Domain.Identity;

namespace Notes.Application.Identity;

public interface ITokenHandler
{
    Task<TokenResponse> GenerateToken(IdentityUser user);
    ClaimsPrincipal GetPrincipalFromToken(string token);
    DateTime GetExpirationTime(long expiryDateUnix);
}

public class TokenHandler : ITokenHandler
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IDataContext _dataContext;
    

    public TokenHandler(JwtConfiguration jwtConfiguration, TokenValidationParameters tokenValidationParameters, IDataContext dataContext)
    {
        _jwtConfiguration = jwtConfiguration;
        _tokenValidationParameters = tokenValidationParameters;
        _dataContext = dataContext;
    }

    public async Task<TokenResponse> GenerateToken(IdentityUser user)
    {
        var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtClaimNames.Sub, user.UserName),
                new Claim(JwtClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtClaimNames.Email, user.Email),
                new Claim(JwtClaimNames.UserId, user.Id)
            }),
            Expires = DateTime.Now.Add(TimeSpan.Parse(_jwtConfiguration.TokenLifetime)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = new RefreshToken
        {
            JwtId = securityToken.Id,
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddMonths(1)
        };
        await _dataContext.RefreshTokens.AddAsync(refreshToken);
        await _dataContext.SaveChangesAsync();
        return new TokenResponse
        {
            Token = tokenHandler.WriteToken(securityToken),
            RefreshToken = refreshToken
        };
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }

            return claimsPrincipal;
        }
        catch
        {
            return null;
        }
    }

    public DateTime GetExpirationTime(long expiryDateUnix)
    {
        var timezoneDifference = DateTime.Now.Subtract(DateTime.UtcNow);
        var dateToExpire = DateTime.UnixEpoch.AddSeconds(expiryDateUnix);
        return timezoneDifference >= TimeSpan.Zero
            ? dateToExpire.Add(timezoneDifference)
            : dateToExpire.Subtract(timezoneDifference);
    }

    private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }
}