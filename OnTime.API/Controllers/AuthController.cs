using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;

namespace OnTime.API.Controllers;

[Authorize]
public class AuthController(SignInManager<User> signInManager, UserManager<User> userManager) : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {

        // Check if user already exists
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest("User with this email already exists.");
        }

        // Validate organization if user is a professional and organizationId is provided
        if (request.IsProfessional && request.OrganizationId.HasValue)
        {
            // Note: You might want to validate that the organization exists
            // This would require injecting the DbContext and checking the organization
        }

        // Create new user
        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLower(),
            UserName = request.Email.Trim().ToLower(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
            IsProfessional = request.IsProfessional,
            OrganizationId = request.OrganizationId
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest($"Failed to create user: {errors}");
        }

        // Sign in the user
        await signInManager.SignInAsync(user, isPersistent: false);

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<ActionResult> Login(LoginRequest request)
    {

        // Find user by email
        var user = await userManager.FindByEmailAsync(request.Email.Trim().ToLower());
        if (user == null)
        {
            return BadRequest("Invalid email or password.");
        }

        // Check password
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return BadRequest("Invalid email or password.");
        }

        // Sign in the user
        await signInManager.SignInAsync(user, isPersistent: false);

        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}