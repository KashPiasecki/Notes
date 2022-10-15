using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Notes.Application.Common.Interfaces.Providers;
using Notes.Domain.Contracts.Constants;

namespace Notes.Infrastructure.Utility.Providers;

public class ClaimsPrincipalInfoProvider : IClaimsPrincipalInfoProvider
{
    public DateTime GetExpiryTime(ClaimsPrincipal claimsPrincipal)
    {
        var expiryDateUnix = long.Parse(claimsPrincipal.Claims.Single(x => x.Type.Equals(JwtRegisteredClaimNames.Exp)).Value);
        var timezoneDifference = DateTime.Now.Subtract(DateTime.UtcNow);
        var dateToExpire = DateTime.UnixEpoch.AddSeconds(expiryDateUnix);
        return dateToExpire.Add(timezoneDifference);
    }

    public string GetId(ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Claims.Single(x => x.Type.Equals(JwtClaimNames.Jti)).Value;

    public string GetUserId(ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Claims.Single(x => x.Type.Equals(JwtClaimNames.UserId)).Value;

}