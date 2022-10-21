using Microsoft.AspNetCore.Identity;
using Notes.Application.Common.Interfaces.Wrappers;

namespace Notes.Infrastructure.Utility.Wrappers;

public class UserManagerWrapper : IUserManagerWrapper
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserManagerWrapper(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityUser?> FindByEmailAsync(string email) =>
        await _userManager.FindByEmailAsync(email);


    public async Task<bool> CheckPasswordAsync(IdentityUser user, string password) =>
        await _userManager.CheckPasswordAsync(user, password);

    public async Task<IdentityUser> FindByIdAsync(string id) =>
        await _userManager.FindByIdAsync(id);

    public IdentityUser CreateIdentityUser(string email, string userName) => 
        new() { Email = email, UserName = userName };

    public async Task<IdentityResult> CreateAsync(IdentityUser user, string password) =>
        await _userManager.CreateAsync(user, password);

    public Task<bool> IsInRoleAsync(IdentityUser user, string role) =>
        _userManager.IsInRoleAsync(user, role);

    public async Task AddToRoleAsync(IdentityUser user, string role) =>
        await _userManager.AddToRoleAsync(user, role);

    public bool HasAnyUsers() =>
        _userManager.Users.Any();
}