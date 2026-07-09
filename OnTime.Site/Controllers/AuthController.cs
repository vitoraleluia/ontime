using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OnTime.Site.Constants;
using OnTime.Site.Models;
using OnTime.Site.ViewModels;

namespace OnTime.Site.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel { PageTitle = "Registar - On Time" });
    }

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

        await this.signInManager.SignInAsync(user, true);
        return string.IsNullOrWhiteSpace(returnUrl)
            ? Redirect("/")
            : LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel { PageTitle = "Entrar - On Time" });
    }

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
        var properties = signInManager.ConfigureExternalAuthenticationProperties(ExternalProviderNames.Google, redirectUrl);
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

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ModelState.AddModelError(string.Empty, "Erro ao carregar informações do login externo.");
            return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
        }

        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
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
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var names = name?.Split(' ', 2) ?? new[] { ExternalProviderNames.Google, "User" };
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = names[0],
                    LastName = names.Length > 1 ? names[1] : string.Empty,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
                }
            }

            var loginResult = await userManager.AddLoginAsync(user, info);
            if (loginResult.Succeeded || userManager.GetLoginsAsync(user).Result.Any(l => l.LoginProvider == info.LoginProvider))
            {
                await signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
        }

        ModelState.AddModelError(string.Empty, "Não foi possível associar a sua conta do Google.");
        return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
    }

}