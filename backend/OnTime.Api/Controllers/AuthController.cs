using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OnTime.Api.Domain.Settings;
using OnTime.Api.Extensions;
using OnTime.Application.Services;
using OnTime.Domain.Enums;
using OnTime.Identity.Constants;
using OnTime.Identity.Entities;

namespace OnTime.Api.Controllers;

[Route("api/auth")]
public class AuthController : BaseApiController
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IIdentityService identityService;
    private readonly AuthenticationSettings authenticationSettings;

    public AuthController(
        ILogger<BaseApiController> logger,
        IMediator mediator,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IIdentityService identityService,
        IOptions<AuthenticationSettings> authenticationOptions) : base(logger, mediator)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.identityService = identityService;
        this.authenticationSettings = authenticationOptions.Value;
    }

    [HttpGet("login/google")]
    public IActionResult GoogleLogin([FromQuery] string? returnUrl)
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", new { returnUrl });
        var properties =
            this.signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme,
                redirectUrl);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl)
    {
        var info = await this.signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return Redirect("/login?error=GoogleAuthFailed".BuildFrontendUrl());
        }

        var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
            isPersistent: true, bypassTwoFactor: true);
        if (!result.Succeeded)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Redirect("/login?error=EmailMissing".BuildFrontendUrl());
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

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return Redirect(returnUrl.BuildFrontendUrl());
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

        var assigned = await this.identityService.AssignRoleAsync(userId, UserRole.Professional);
        if (!assigned)
        {
            return BadRequest("Utilizador não encontrado ou falha ao atribuir o papel.");
        }

        return Ok();
    }
}