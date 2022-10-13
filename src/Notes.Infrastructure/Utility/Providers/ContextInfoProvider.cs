using Microsoft.AspNetCore.Http;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Contracts.Constants;

namespace Notes.Infrastructure.Utility.Providers;

public class ContextInfoProvider : IContextInfoProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext!.User.Claims.Single(x => x.Type.Equals(JwtClaimNames.UserId)).Value;
    }

    public string GetRoute()
    {
        return _httpContextAccessor.HttpContext!.Request.Path;
    }
}