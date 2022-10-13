using Microsoft.AspNetCore.Http;
using Notes.Domain.Contracts.Constants;

namespace Notes.Infrastructure.Utility.Extensions;

public static class HttpContextExtension
{
    public static string GetUserId(this HttpContext httpContext)
    {
        return httpContext.User.Claims.Single(x => x.Type.Equals(JwtClaimNames.UserId)).Value;
    }
}