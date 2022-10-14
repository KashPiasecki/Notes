using Microsoft.AspNetCore.Http;
using Notes.Domain.Contracts.Constants;

namespace Notes.Infrastructure.Utility.Extensions;

public static class HttpContextExtension
{
    public static string GetUserId(this HttpContext httpContext) =>
        httpContext.User.Claims.SingleOrDefault(x => x.Type.Equals(JwtClaimNames.UserId))!.Value;
}