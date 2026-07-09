using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OnTime.Site.Constants;
using OnTime.Site.Models;
using OnTime.Site.ViewModels;

namespace OnTime.Site.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IEmailSender<ApplicationUser> emailSender;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender<ApplicationUser> emailSender)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.emailSender = emailSender;
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel { PageTitle = "Registar - On Time" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return PartialView(ViewNames.RegistrationForm, model);

        if (model.Password != model.PasswordConfirmation)
        {
            ModelState.AddModelError(nameof(model.PasswordConfirmation), "As passwords não coincidem.");
            return PartialView(ViewNames.RegistrationForm, model);
        }

        var existingUser = await this.userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Este e-mail já está em uso.");
            return PartialView(ViewNames.RegistrationForm, model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber
        };
        var identityResult = await this.userManager.CreateAsync(user, model.Password);
        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return PartialView(ViewNames.RegistrationForm, model);
        }

        // Generate confirmation token and link
        var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Action(
            nameof(ConfirmEmail),
            "Auth",
            new { userId = user.Id, code },
            protocol: HttpContext.Request.Scheme);

        await this.emailSender.SendConfirmationLinkAsync(user, model.Email, callbackUrl!);

        return PartialView(ViewNames.RegisterSuccess, model.Email);
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel { PageTitle = "Entrar - On Time" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return PartialView(ViewNames.LoginForm, model);

        var signInResult = await this.signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            true,
            false);

        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "A sua conta está bloqueada.");
            }
            else if (signInResult.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "A sua conta ainda não foi confirmada. Verifique o seu e-mail ou clique no link abaixo para reenviar.");
                model.IsResendConfirmationVisible = true;
                model.UnconfirmedEmail = model.Email;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "E-mail ou palavra-passe incorretos.");
            }

            return PartialView(ViewNames.LoginForm, model);
        }

        return string.IsNullOrWhiteSpace(returnUrl)
            ? Redirect("/")
            : LocalRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await this.signInManager.SignOutAsync();
        return string.IsNullOrWhiteSpace(returnUrl)
            ? Redirect("/")
            : LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", new { returnUrl });
        var properties =
            this.signInManager.ConfigureExternalAuthenticationProperties(ExternalProviderNames.Google, redirectUrl);
        return Challenge(properties, ExternalProviderNames.Google);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Erro do Google: {remoteError}");
            return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
        }

        var info = await this.signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ModelState.AddModelError(string.Empty, "Erro ao carregar informações do login externo.");
            return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
        }

        var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
            isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "A sua conta está bloqueada.");
            return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
        }

        // If the user does not have an account, create it automatically.
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name);

        if (email != null)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var names = name?.Split(' ', 2) ?? [ExternalProviderNames.Google, "User"];
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
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
                }
            }

            var loginResult = await this.userManager.AddLoginAsync(user, info);
            if (loginResult.Succeeded ||
                this.userManager.GetLoginsAsync(user).Result.Any(l => l.LoginProvider == info.LoginProvider))
            {
                await this.signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
        }

        ModelState.AddModelError(string.Empty, "Não foi possível associar a sua conta do Google.");
        return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await this.userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Não foi possível carregar o utilizador com o ID '{userId}'.");
        }

        var result = await this.userManager.ConfirmEmailAsync(user, code);
        var success = result.Succeeded;
        if (success)
        {
            await this.userManager.ResetAccessFailedCountAsync(user);
        }

        var model = new ConfirmEmailViewModel
        {
            PageTitle = "Confirmar E-mail - On Time",
            IsSucceeded = success,
            Email = user.Email ?? string.Empty
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel { PageTitle = "Recuperar Palavra-passe - On Time" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return PartialView(ViewNames.ForgotPasswordForm, model);

        var user = await this.userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await this.userManager.IsEmailConfirmedAsync(user)))
        {
            return PartialView(ViewNames.ForgotPasswordFailed);
        }

        if (await this.userManager.IsLockedOutAsync(user))
        {
            ModelState.AddModelError(string.Empty, "A sua conta está bloqueada devido a excesso de tentativas. Por favor, tente mais tarde.");
            return PartialView(ViewNames.ForgotPasswordForm, model);
        }

        await this.userManager.AccessFailedAsync(user);
        if (await this.userManager.IsLockedOutAsync(user))
        {
            ModelState.AddModelError(string.Empty, "A sua conta foi bloqueada devido a excesso de tentativas. Por favor, tente mais tarde.");
            return PartialView(ViewNames.ForgotPasswordForm, model);
        }

        var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(
            nameof(ResetPassword),
            "Auth",
            new { code },
            protocol: HttpContext.Request.Scheme);

        await this.emailSender.SendPasswordResetLinkAsync(user, model.Email, callbackUrl!);

        return PartialView(ViewNames.ForgotPasswordSuccess);
    }

    [HttpGet]
    public IActionResult ResetPassword(string? code = null)
    {
        return code == null
            ? BadRequest("É necessário um código para repor a palavra-passe.")
            : View(new ResetPasswordViewModel
            {
                PageTitle = "Repor Palavra-passe - On Time",
                Code = code
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return PartialView(ViewNames.ResetPasswordForm, model);

        var user = await this.userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return PartialView(ViewNames.ResetPasswordSuccess);
        }

        var result = await this.userManager.ResetPasswordAsync(user, model.Code, model.Password);
        if (result.Succeeded)
        {
            await this.userManager.ResetAccessFailedCountAsync(user);
            return PartialView(ViewNames.ResetPasswordSuccess);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return PartialView(ViewNames.ResetPasswordForm, model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("E-mail inválido.");
        }

        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest("Utilizador não registado.");
        }

        if (await this.userManager.IsLockedOutAsync(user))
        {
            ModelState.AddModelError(string.Empty, "A sua conta está bloqueada devido a excesso de tentativas. Por favor, tente mais tarde.");
            return PartialView(ViewNames.ResendEmailConfirmationError);
        }

        if (!(await this.userManager.IsEmailConfirmedAsync(user)))
        {
            await this.userManager.AccessFailedAsync(user);
            if (await this.userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError(string.Empty, "A sua conta foi bloqueada devido a excesso de tentativas. Por favor, tente mais tarde.");
                return PartialView(ViewNames.ResendEmailConfirmationError);
            }

            var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
                nameof(ConfirmEmail),
                "Auth",
                new { userId = user.Id, code },
                protocol: HttpContext.Request.Scheme);
            await this.emailSender.SendConfirmationLinkAsync(user, email, callbackUrl!);
        }

        return PartialView(ViewNames.ResendEmailConfirmationSuccess);
    }

    [HttpGet]
    public IActionResult AccessDenied() => View(ViewNames.AccessDenied, new BaseViewModel { PageTitle = "Acesso Recusado - On Time" });
}