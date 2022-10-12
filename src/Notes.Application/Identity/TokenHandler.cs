using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Configurations;
using Notes.Domain.Contracts;
using Notes.Domain.Contracts.Identity;

namespace Notes.Application.Identity;

public interface ITokenHandler
{
    Task<TokenResponse> GenerateToken(IdentityUser user);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
    DateTime GetExpirationTime(long expiryDateUnix);
}

public class TokenHandler : ITokenHandler
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IDataContext _dataContext;
    private readonly ILogger<TokenHandler> _logger;


    public TokenHandler(JwtConfiguration jwtConfiguration, TokenValidationParameters tokenValidationParameters, IDataContext dataContext,
        ILogger<TokenHandler> logger, UserManager<IdentityUser> userManager)
    {
        _jwtConfiguration = jwtConfiguration;
        _tokenValidationParameters = tokenValidationParameters;
        _dataContext = dataContext;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<TokenResponse> GenerateToken(IdentityUser user)
    {
        _logger.LogInformation("Attempt to create token for {Email}", user.Email);
        var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);

        var claims = new List<Claim>
        {
            new (JwtClaimNames.Sub, user.UserName),
            new (JwtClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtClaimNames.Email, user.Email),
            new (JwtClaimNames.UserId, user.Id),
            new (ClaimTypes.Role, RoleNames.User)
        };
        
        if (await _userManager.IsInRoleAsync(user, RoleNames.Admin))
        {
            claims.Add(new Claim(ClaimTypes.Role, RoleNames.Admin));
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
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
        _logger.LogInformation("Successfully created token for {Email}", user.Email);
        return new TokenResponse
        {
            Token = tokenHandler.WriteToken(securityToken),
            RefreshToken = refreshToken
        };
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
            if (IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return claimsPrincipal;
            }

            _logger.LogError("Jwt without valid security algorithm!");
            throw new JwtInvalidSecurityAlgorithmException();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Token validation exception");
            return null;
        }
    }

    public DateTime GetExpirationTime(long expiryDateUnix)
    {
        var timezoneDifference = DateTime.Now.Subtract(DateTime.UtcNow);
        var dateToExpire = DateTime.UnixEpoch.AddSeconds(expiryDateUnix);
        return dateToExpire.Add(timezoneDifference);
    }

    private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken) =>
        (validatedToken is JwtSecurityToken jwtSecurityToken) &&
        jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
}