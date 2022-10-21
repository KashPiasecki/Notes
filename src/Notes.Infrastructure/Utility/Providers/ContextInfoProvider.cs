using Microsoft.AspNetCore.Http;
using Notes.Application.Common.Interfaces.Providers;
using Notes.Infrastructure.Utility.Extensions;

namespace Notes.Infrastructure.Utility.Providers;

public class ContextInfoProvider : IContextInfoProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId() =>
        _httpContextAccessor.HttpContext!.GetUserId();

    public string GetRoute() =>
        _httpContextAccessor.HttpContext!.Request.Path;
}