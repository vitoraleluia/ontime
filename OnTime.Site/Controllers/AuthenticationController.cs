using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OnTime.Site.ViewModels;

namespace OnTime.Site.Controllers;

public class AuthenticationController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public AuthenticationController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return PartialView("_RegisterForm", model);

        if (model.Password != model.PasswordConfirmation)
        {
            ModelState.AddModelError(nameof(model.PasswordConfirmation), "As passwords não coicidem.");
            return PartialView("_RegisterForm", model);
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var identityResult = await this.userManager.CreateAsync(user, model.Password);
        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                return PartialView("_RegisterForm", model);
            }
        }

        await this.signInManager.SignInAsync(user, true);
        return string.IsNullOrWhiteSpace(returnUrl)
            ? Redirect("/")
            : LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return PartialView("_LoginForm", model);

        var signInResult = await this.signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            true,
            false);

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Ocorreu um erro.");
            return PartialView("_LoginForm", model);
        }

        if (!signInResult.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "A sua conta está bloqueada.");
            return PartialView("_LoginForm", model);
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
}