using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Configurations;
using Notes.Domain.Contracts;

namespace Notes.Infrastructure.Identity;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtConfiguration _jwtConfiguration;

    public TokenGenerator(JwtConfiguration jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration;
    }

    public string GenerateToken(IdentityUser user)
    {
        var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtClaimNames.UserName, user.UserName),
                new Claim(JwtClaimNames.TokenId, Guid.NewGuid().ToString()),
                new Claim(JwtClaimNames.Email, user.Email),
                new Claim(JwtClaimNames.Id, user.Id)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
}