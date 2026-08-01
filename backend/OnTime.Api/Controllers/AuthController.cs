using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OnTime.Identity.Constants;
using OnTime.Identity.Entities;

namespace OnTime.Api.Controllers;

[Route("api/auth")]
public class AuthController : BaseApiController
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;

    public AuthController(
        ILogger<BaseApiController> logger,
        IMediator mediator,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager) : base(logger, mediator)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    [HttpGet("login/google")]
    public IActionResult GoogleLogin([FromQuery] string? returnUrl)
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", new { returnUrl });
        var properties = this.signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl)
    {
        var info = await this.signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return Redirect("/login?error=GoogleAuthFailed");
        }

        var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
        if (!result.Succeeded)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Redirect("/login?error=EmailMissing");
            }

            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                await this.userManager.CreateAsync(user);
            }

            await this.userManager.AddLoginAsync(user, info);
            await this.signInManager.SignInAsync(user, isPersistent: true);
        }

        return Redirect(returnUrl ?? "/");
    }

    [Authorize]
    [HttpPost("assign-professional")]
    public async Task<IActionResult> AssignProfessionalRole()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await this.userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("Utilizador não encontrado.");
        }

        if (!await this.userManager.IsInRoleAsync(user, IdentityRoles.Professional))
        {
            await this.userManager.AddToRoleAsync(user, IdentityRoles.Professional);
        }

        return Ok();
    }
}
