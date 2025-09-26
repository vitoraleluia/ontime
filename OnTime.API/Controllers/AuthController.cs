using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OnTime.API.Models.Domain;

namespace OnTime.API.Controllers;

[Authorize]
public class AuthController(SignInManager<User> signInManager, UserManager<User> userManager) : BaseApiController
{
    [AllowAnonymous]
    public async Task<ActionResult> Register()
    {
        var result = await userManager.CreateAsync(new Models.Domain.User());
        if (!result.Succeeded)
            return BadRequest();

        return Ok();
    }

    [AllowAnonymous]
    public async Task<ActionResult> Login()
    {
        var result = await userManager.CreateAsync(new User());
        if (!result.Succeeded)
            return BadRequest();

        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}