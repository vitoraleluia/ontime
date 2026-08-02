using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OnTime.Api.Extensions;
using OnTime.Application.Features.UserProfile.Commands;
using OnTime.Identity.Constants;
using OnTime.Identity.Entities;

namespace OnTime.Api.Controllers;

public class GoogleAuthController : BaseApiController
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;

    public GoogleAuthController(
        ILogger<BaseApiController> logger,
        IMediator mediator,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager) : base(logger, mediator)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    [HttpGet("login")]
    public IActionResult GoogleLogin([FromQuery] string? returnUrl)
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "GoogleAuth", new { returnUrl });
        var properties =
            this.signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme,
                redirectUrl);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl)
    {
        var info = await this.signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return Redirect("/login?error=GoogleAuthFailed".BuildFrontendUrl());
        }

        var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
            isPersistent: true, bypassTwoFactor: true);

        ApplicationUser? user = null;
        if (!result.Succeeded)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Redirect("/login?error=EmailMissing".BuildFrontendUrl());
            }

            user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                await this.userManager.CreateAsync(user);
            }

            await this.userManager.AddLoginAsync(user, info);
            await this.signInManager.SignInAsync(user, isPersistent: true);
        }
        else
        {
            user = await this.userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        }

        if (user != null)
        {
            var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName)
                            ?? info.Principal.FindFirstValue("given_name");
            var familyName = info.Principal.FindFirstValue(ClaimTypes.Surname)
                             ?? info.Principal.FindFirstValue("family_name");

            if (string.IsNullOrWhiteSpace(givenName))
            {
                var fullName = info.Principal.FindFirstValue(ClaimTypes.Name)
                               ?? info.Principal.FindFirstValue("name");
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    var parts = fullName.Split(' ', 2);
                    givenName = parts[0];
                    familyName = string.IsNullOrWhiteSpace(familyName) && parts.Length > 1 ? parts[1] : familyName;
                }
            }

            givenName = string.IsNullOrWhiteSpace(givenName) ? user.Email!.Split('@')[0] : givenName;
            familyName = string.IsNullOrWhiteSpace(familyName) ? "Utilizador" : familyName;

            await this.Mediator.Send(new CreateUserProfileCommand(user.Id, user.Email!, givenName, familyName));
        }

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return Redirect(returnUrl.BuildFrontendUrl());
    }
}