using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OnTime.Application.Services;
using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;

namespace OnTime.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string PasswordConfirmation,
    string FirstName,
    string LastName,
    string? PhoneNumber) : IRequest<Result>;

public class RegisterCommandHandler : BaseHandler<RegisterCommand, Result>
{
    private readonly IIdentityService identityService;

    public RegisterCommandHandler(
        IIdentityService identityService,
        ILogger<RegisterCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result> HandleSafe(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (request.Password != request.PasswordConfirmation)
        {
            return Result.Failure(new Error("PasswordMismatch", "As passwords não coincidem."));
        }

        return await this.identityService.Register(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber);
    }
}
