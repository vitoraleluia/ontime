using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

public class ForgotPasswordCommandHandler : BaseHandler<ForgotPasswordCommand, Result>
{
    private readonly IIdentityService identityService;

    public ForgotPasswordCommandHandler(
        IIdentityService identityService,
        ILogger<ForgotPasswordCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result> HandleSafe(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        return await this.identityService.ForgotPassword(request.Email);
    }
}