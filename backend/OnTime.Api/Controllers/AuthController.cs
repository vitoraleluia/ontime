using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OnTime.Api.Extensions;
using OnTime.Api.Models.Auth;
using OnTime.Application.Features.Auth.Commands;
using OnTime.Application.Services;
using OnTime.Domain.Common;
using OnTime.Domain.Enums;
using OnTime.Domain.Settings;
using OnTime.Identity.Constants;
using OnTime.Identity.Entities;

namespace OnTime.Api.Controllers;

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

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand(request.Email, request.Password, request.FirstName, request.LastName,
            request.PhoneNumber);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok();
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutUserCommand();
        await this.Mediator.Send(command);
        return Ok();
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok();
    }

    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var command = new ConfirmEmailCommand(request.UserId, request.Token);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok();
    }

    [HttpPost("resend-confirmation-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
    {
        var command = new ResendConfirmationEmailCommand(request.Email);
        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok();
    }
}