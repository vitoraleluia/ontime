using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnTime.Api.Models.Account;
using OnTime.Application.Domain.Constants;
using OnTime.Application.Features.UserProfile.Commands;
using OnTime.Application.Features.UserProfile.Queries;
using OnTime.Application.Features.UserProfile.Responses;
using OnTime.Application.Services;
using OnTime.Domain.Common;
using OnTime.Domain.Enums;
using OnTime.Identity.Constants;

namespace OnTime.Api.Controllers;

[Authorize]
public class AccountController : BaseApiController
{
    private readonly IIdentityService identityService;

    public AccountController(
        ILogger<BaseApiController> logger,
        IMediator mediator,
        IIdentityService identityService) : base(logger, mediator)
    {
        this.identityService = identityService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> GetCurrentProfile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(ClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return HandleFailure(ErrorCode.Unauthorized, "ID de utilizador ausente no token.", StatusCodes.Status401Unauthorized);
        }

        var query = new GetCurrentUserProfileQuery(userId);
        var result = await this.Mediator.Send(query);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        var response = result.Value!;
        response.IsProfessional = await this.identityService.IsInRole(userId, UserRole.Professional);
        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(ClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return HandleFailure(ErrorCode.Unauthorized, "ID de utilizador ausente no token.", StatusCodes.Status401Unauthorized);
        }

        var command = new UpdateUserProfileCommand(userId, request.FirstName, request.LastName, request.PhoneNumber, request.ProfilePictureId);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        var response = result.Value!;
        response.IsProfessional = await this.identityService.IsInRole(userId, UserRole.Professional);
        return Ok(response);
    }

    [HttpPost("assign-professional")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> AssignProfessional()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(ClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return HandleFailure(ErrorCode.Unauthorized, "ID de utilizador ausente no token.", StatusCodes.Status401Unauthorized);
        }

        var assigned = await this.identityService.AssignRole(userId, UserRole.Professional);
        if (!assigned)
        {
            return HandleFailure(ErrorCode.BadRequest, "Falha ao atribuir o papel profissional.");
        }

        var query = new GetCurrentUserProfileQuery(userId);
        var result = await this.Mediator.Send(query);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        var response = result.Value!;
        response.IsProfessional = true;
        return Ok(response);
    }
}