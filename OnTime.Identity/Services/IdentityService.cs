using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Identity;

using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;
using OnTime.Application.Services;
using OnTime.Identity.Entities;

namespace OnTime.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IEmailSender<ApplicationUser> emailSender;
    private readonly ICallbackUrlGenerator urlGenerator;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender<ApplicationUser> emailSender,
        ICallbackUrlGenerator urlGenerator)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.emailSender = emailSender;
        this.urlGenerator = urlGenerator;
    }

    public async Task<Result> Register(
        string email,
        string password,
        string firstName,
        string lastName,
        string? phoneNumber)
    {
        var existingUser = await this.userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return Result.Failure(new Error("EmailInUse", "Este e-mail já está em uso."));
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber
        };

        var result = await this.userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Erro ao criar utilizador.";
            return Result.Failure(new Error("RegistrationFailed", firstError));
        }

        var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = this.urlGenerator.GetConfirmEmailUrl(user.Id, code);

        await this.emailSender.SendConfirmationLinkAsync(user, email, callbackUrl);

        return Result.Success();
    }

    public async Task<Result<LoginStatus>> Login(string email, string password)
    {
        var result = await this.signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return Result<LoginStatus>.Success(LoginStatus.Success);
        }

        if (result.IsLockedOut)
        {
            return Result<LoginStatus>.Success(LoginStatus.LockedOut);
        }

        if (result.IsNotAllowed)
        {
            return Result<LoginStatus>.Success(LoginStatus.NotAllowed);
        }

        return Result<LoginStatus>.Failure(new Error("InvalidCredentials", "E-mail ou palavra-passe incorretos."));
    }

    public async Task<Result> Logout()
    {
        await this.signInManager.SignOutAsync();
        return Result.Success();
    }

    public async Task<Result<string>> ConfirmEmail(string userId, string code)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<string>.Failure(new Error("UserNotFound", $"Não foi possível encontrar o utilizador com o ID '{userId}'."));
        }

        var result = await this.userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Erro ao confirmar e-mail.";
            return Result<string>.Failure(new Error("ConfirmationFailed", firstError));
        }

        await this.userManager.ResetAccessFailedCountAsync(user);
        return Result<string>.Success(user.Email ?? string.Empty);
    }

    public async Task<Result> ForgotPassword(string email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null || !(await this.userManager.IsEmailConfirmedAsync(user)))
        {
            // Replicate original code behavior (sends custom fail response but doesn't throw)
            return Result.Failure(new Error("ForgotPasswordFailed", "ForgotPasswordFailed"));
        }

        if (await this.userManager.IsLockedOutAsync(user))
        {
            return Result.Failure(new Error("LockedOut", "A sua conta está bloqueada devido a excesso de tentativas. Por favor, tente mais tarde."));
        }

        await this.userManager.AccessFailedAsync(user);
        if (await this.userManager.IsLockedOutAsync(user))
        {
            return Result.Failure(new Error("LockedOut", "A sua conta foi bloqueada devido a excesso de tentativas. Por favor, tente mais tarde."));
        }

        var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = this.urlGenerator.GetResetPasswordUrl(code);

        await this.emailSender.SendPasswordResetLinkAsync(user, email, callbackUrl);

        return Result.Success();
    }

    public async Task<Result> ResetPassword(string email, string code, string password)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Decoupled Success to prevent user enumeration
            return Result.Success();
        }

        var result = await this.userManager.ResetPasswordAsync(user, code, password);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Erro ao repor a palavra-passe.";
            return Result.Failure(new Error("ResetPasswordFailed", firstError));
        }

        await this.userManager.ResetAccessFailedCountAsync(user);
        return Result.Success();
    }

    public async Task<Result> ResendEmailConfirmation(string email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result.Failure(new Error("UserNotFound", "Utilizador não registado."));
        }

        if (await this.userManager.IsLockedOutAsync(user))
        {
            return Result.Failure(new Error("LockedOut", "A sua conta está bloqueada devido a excesso de tentativas. Por favor, tente mais tarde."));
        }

        if (!(await this.userManager.IsEmailConfirmedAsync(user)))
        {
            await this.userManager.AccessFailedAsync(user);
            if (await this.userManager.IsLockedOutAsync(user))
            {
                return Result.Failure(new Error("LockedOut", "A sua conta foi bloqueada devido a excesso de tentativas. Por favor, tente mais tarde."));
            }

            var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = this.urlGenerator.GetConfirmEmailUrl(user.Id, code);
            await this.emailSender.SendConfirmationLinkAsync(user, email, callbackUrl);
        }

        return Result.Success();
    }

    public async Task<Result<string>> ProcessGoogleCallback(string? returnUrl, string? remoteError)
    {
        returnUrl ??= "/";
        if (remoteError != null)
        {
            return Result<string>.Failure(new Error("GoogleAuthError", $"Erro do Google: {remoteError}"));
        }

        var info = await this.signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return Result<string>.Failure(new Error("ExternalLoginInfoError", "Erro ao carregar informações do login externo."));
        }

        var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
            isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            return Result<string>.Success(returnUrl);
        }

        if (result.IsLockedOut)
        {
            return Result<string>.Failure(new Error("LockedOut", "A sua conta está bloqueada."));
        }

        // If the user does not have an account, create it automatically.
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name);

        if (email != null)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var names = name?.Split(' ', 2) ?? ["Google", "User"];
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = names[0],
                    LastName = names.Length > 1 ? names[1] : string.Empty,
                    EmailConfirmed = true
                };
                var createResult = await this.userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var firstError = createResult.Errors.FirstOrDefault()?.Description ?? "Erro ao criar conta Google.";
                    return Result<string>.Failure(new Error("UserCreationError", firstError));
                }
            }

            var loginResult = await this.userManager.AddLoginAsync(user, info);
            if (loginResult.Succeeded ||
                this.userManager.GetLoginsAsync(user).Result.Any(l => l.LoginProvider == info.LoginProvider))
            {
                await this.signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                return Result<string>.Success(returnUrl);
            }
        }

        return Result<string>.Failure(new Error("GoogleAssociationError", "Não foi possível associar a sua conta do Google."));
    }
}