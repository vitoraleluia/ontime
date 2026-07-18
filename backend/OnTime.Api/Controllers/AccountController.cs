using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnTime.Api.Models.Account;
using OnTime.Application.Common.Constants;
using OnTime.Application.Features.UserProfile.Commands;
using OnTime.Application.Features.UserProfile.Queries;
using OnTime.Application.Features.UserProfile.Responses;
namespace OnTime.Api.Controllers;

[Authorize]
public class AccountController : BaseApiController
{
    public AccountController(ILogger<BaseApiController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> GetCurrentProfile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(ClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("ID de utilizador ausente no token.");
        }

        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? User.FindFirst(ClaimNames.Email)?.Value
                    ?? string.Empty;

        var givenName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value
                        ?? User.FindFirst(ClaimNames.GivenName)?.Value
                        ?? string.Empty;

        var familyName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value
                         ?? User.FindFirst(ClaimNames.FamilyName)?.Value
                         ?? string.Empty;

        // If Name claims are empty, extract fallback from email or display name
        if (string.IsNullOrEmpty(givenName))
        {
            var fullName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                           ?? User.FindFirst(ClaimNames.Name)?.Value
                           ?? User.FindFirst(ClaimNames.PreferredUsername)?.Value
                           ?? email.Split('@')[0];

            var nameParts = fullName.Split(' ', 2);
            givenName = nameParts[0];
            familyName = nameParts.Length > 1 ? nameParts[1] : "Utilizador";
        }

        var query = new GetCurrentUserProfileQuery(userId, email, givenName, familyName);
        var result = await this.Mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(result.Error?.Message);
        }

        return Ok(result.Value);
    }

    [HttpPut]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(ClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("ID de utilizador ausente no token.");
        }

        var command = new UpdateUserProfileCommand(userId, request.FirstName, request.LastName, request.PhoneNumber, request.ProfilePictureId);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error?.Message);
        }

        return Ok(result.Value);
    }

    [HttpPost("assign-professional")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> AssignProfessional()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(ClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("ID de utilizador ausente no token.");
        }

        var command = new AssignProfessionalCommand(userId);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error?.Message);
        }

        return Ok(result.Value);
    }
}