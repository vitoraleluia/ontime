using Microsoft.AspNetCore.Identity;
using OnTime.Application.Services;
using OnTime.Domain.Enums;
using OnTime.Identity.Constants;
using OnTime.Identity.Entities;

using ApplicationIdentityResult = OnTime.Application.Models.Identity.IdentityResult;

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

    public async Task<bool> IsInRole(string userId, UserRole role, CancellationToken cancellationToken = default)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        return await this.userManager.IsInRoleAsync(user, role.ToString());
    }

    public async Task<bool> AssignRole(string userId, UserRole role, CancellationToken cancellationToken = default)
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

    public async Task<ApplicationIdentityResult> RegisterUser(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await this.userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return ApplicationIdentityResult.Failure(result.Errors.Select(e => e.Description));
        }

        return ApplicationIdentityResult.Success(userId: user.Id);
    }

    public async Task<ApplicationIdentityResult> PasswordSignIn(string email, string password, bool lockoutOnFailure = true, CancellationToken cancellationToken = default)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ApplicationIdentityResult.Failure("Credenciais inválidas. Verifique o email e a palavra-passe.");
        }

        var result = await this.signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: lockoutOnFailure);

        if (result.IsLockedOut)
        {
            return ApplicationIdentityResult.LockedOut("Conta bloqueada temporariamente.");
        }

        if (result.RequiresTwoFactor)
        {
            return new ApplicationIdentityResult { IsSuccess = false, RequiresTwoFactor = true, ErrorMessage = "Autenticação de dois fatores necessária." };
        }

        if (!result.Succeeded)
        {
            return ApplicationIdentityResult.Failure("Credenciais inválidas. Verifique o email e a palavra-passe.");
        }

        return ApplicationIdentityResult.Success(userId: user.Id);
    }

    public async Task<ApplicationIdentityResult> GeneratePasswordResetToken(string email, CancellationToken cancellationToken = default)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ApplicationIdentityResult.Success();
        }

        var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
        return ApplicationIdentityResult.Success(userId: user.Id, token: token);
    }

    public async Task<ApplicationIdentityResult> ResetPassword(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ApplicationIdentityResult.Failure("Utilizador não encontrado.");
        }

        var result = await this.userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            return ApplicationIdentityResult.Failure(result.Errors.Select(e => e.Description));
        }

        return ApplicationIdentityResult.Success(userId: user.Id);
    }

    public async Task<ApplicationIdentityResult> GenerateEmailConfirmationToken(string email, CancellationToken cancellationToken = default)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ApplicationIdentityResult.Success();
        }

        var token = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        return ApplicationIdentityResult.Success(userId: user.Id, token: token);
    }

    public async Task<ApplicationIdentityResult> ConfirmEmail(string userId, string token, CancellationToken cancellationToken = default)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ApplicationIdentityResult.Failure("Utilizador não encontrado.");
        }

        var result = await this.userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return ApplicationIdentityResult.Failure(result.Errors.Select(e => e.Description));
        }

        return ApplicationIdentityResult.Success(userId: user.Id);
    }

    public async Task SignOut(CancellationToken cancellationToken = default)
    {
        await this.signInManager.SignOutAsync();
    }
}
