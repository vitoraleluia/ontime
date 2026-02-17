using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OnTime.API.Constants;
using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;
using OnTime.API.Models.Responses;
using OnTime.API.Models.Settings;
using OnTime.API.Services.Authentication;

namespace OnTime.API.Controllers;

[Authorize]
public class AuthController(
    UserManager<User> userManager,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtSettings> jwtSettings) : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest("User with this email already exists.");
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

        // Generate tokens
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);

        // Set refresh token as HTTP-only cookie
        SetRefreshTokenCookie(refreshToken.Token);

        var userResponse = new UserResponse(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.IsProfessional,
            user.OrganizationId,
            user.PhoneNumber
        );

        return Ok(new AuthResponse(accessToken, userResponse));
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Find user by email
        var user = await userManager.FindByEmailAsync(request.Email.Trim().ToLower());
        if (user == null)
        {
            return BadRequest("Invalid email or password.");
        }

        // Check password
        var result = await userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            return BadRequest("Invalid email or password.");
        }

        // Generate tokens
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);

        // Set refresh token as HTTP-only cookie
        SetRefreshTokenCookie(refreshToken.Token);

        var userResponse = new UserResponse(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.IsProfessional,
            user.OrganizationId,
            user.PhoneNumber
        );

        return Ok(new AuthResponse(accessToken, userResponse));
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<ActionResult<RefreshResponse>> Refresh()
    {
        var refreshTokenValue = Request.Cookies[CookieNames.RefreshToken];
        if (string.IsNullOrEmpty(refreshTokenValue))
        {
            return Unauthorized(new { message = "Refresh token not found" });
        }

        var newRefreshToken = await refreshTokenService.ValidateAndRotateAsync(refreshTokenValue);

        if (newRefreshToken == null)
        {
            // Clear the invalid cookie
            ClearRefreshTokenCookie();
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        // Generate new access token
        var accessToken = jwtService.GenerateAccessToken(newRefreshToken.User);

        // Set new refresh token as HTTP-only cookie
        SetRefreshTokenCookie(newRefreshToken.Token);

        return Ok(new RefreshResponse(accessToken));
    }

    [HttpPost("[action]")]
    public async Task<ActionResult> Logout()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await refreshTokenService.RevokeTokenAsync(userId);
        }

        ClearRefreshTokenCookie();
        return Ok(new { message = "Logged out successfully" });
    }

    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays)
        };

        Response.Cookies.Append(CookieNames.RefreshToken, token, cookieOptions);
    }

    private void ClearRefreshTokenCookie()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        Response.Cookies.Append(CookieNames.RefreshToken, "", cookieOptions);
    }
}

public record AuthResponse(string AccessToken, UserResponse User);
public record RefreshResponse(string AccessToken);