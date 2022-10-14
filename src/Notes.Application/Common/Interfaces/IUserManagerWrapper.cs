using Microsoft.AspNetCore.Identity;

namespace Notes.Application.Common.Interfaces;

public interface IUserManagerWrapper
{
    public Task<IdentityUser?> FindByEmailAsync(string email);
    public Task<bool> CheckPasswordAsync(IdentityUser user, string password);
    Task<IdentityUser> FindByIdAsync(string id);
    Task<IdentityResult> CreateAsync(IdentityUser user, string password);
    Task AddToRoleAsync(IdentityUser user, string role);
    bool HasAnyUsers();
    IdentityUser CreateIdentityUser(string email, string userName);
}