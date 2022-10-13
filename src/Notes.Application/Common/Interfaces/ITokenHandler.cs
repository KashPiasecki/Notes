using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Notes.Domain.Contracts.Responses;

namespace Notes.Application.Common.Interfaces;

public interface ITokenHandler
{
    Task<TokenResponse> GenerateToken(IdentityUser user);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
    DateTime GetExpirationTime(long expiryDateUnix);
}