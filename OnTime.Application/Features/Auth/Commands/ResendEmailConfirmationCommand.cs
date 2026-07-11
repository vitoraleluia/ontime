using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record ResendEmailConfirmationCommand(string Email) : IRequest<Result>;

public class ResendEmailConfirmationCommandHandler : BaseHandler<ResendEmailConfirmationCommand, Result>
{
    private readonly IIdentityService identityService;

    public ResendEmailConfirmationCommandHandler(
        IIdentityService identityService,
        ILogger<ResendEmailConfirmationCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result> HandleSafe(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        return await this.identityService.ResendEmailConfirmation(request.Email);
    }
}