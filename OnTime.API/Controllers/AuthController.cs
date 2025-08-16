using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OnTime.API.Models.Domain;

namespace OnTime.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly SignInManager<User> signInManager;

    public AuthController(SignInManager<User> signInManager)
    {
        this.signInManager = signInManager;
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}