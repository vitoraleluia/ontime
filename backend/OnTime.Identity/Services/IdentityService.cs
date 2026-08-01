using Microsoft.AspNetCore.Identity;
using OnTime.Application.Services;
using OnTime.Domain.Enums;
using OnTime.Identity.Entities;

namespace OnTime.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public async Task<bool> IsInRoleAsync(string userId, UserRole role, CancellationToken cancellationToken = default)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        return await this.userManager.IsInRoleAsync(user, role.ToString());
    }

    public async Task<bool> AssignRoleAsync(string userId, UserRole role, CancellationToken cancellationToken = default)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var roleName = role.ToString();
        if (!await this.userManager.IsInRoleAsync(user, roleName))
        {
            var result = await this.userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                await this.signInManager.RefreshSignInAsync(user);
                return true;
            }

            return false;
        }

        return true;
    }
}
