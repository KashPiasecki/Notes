using System.Security.Claims;

namespace Notes.Application.Common.Interfaces.Providers;

public interface IClaimsPrincipalInfoProvider
{
    DateTime GetExpiryTime(ClaimsPrincipal claimsPrincipal);
    string GetId(ClaimsPrincipal claimsPrincipal);
    string GetUserId(ClaimsPrincipal claimsPrincipal);
}