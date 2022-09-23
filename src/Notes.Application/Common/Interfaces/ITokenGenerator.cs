using Microsoft.AspNetCore.Identity;

namespace Notes.Application.Common.Interfaces;

public interface ITokenGenerator
{
    public string GenerateToken(IdentityUser user);
}