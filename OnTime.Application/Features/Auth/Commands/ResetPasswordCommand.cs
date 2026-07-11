using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OnTime.Application.Services;
using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;

namespace OnTime.Application.Features.Auth.Commands;

public record ResetPasswordCommand(
    string Email,
    string Code,
    string Password) : IRequest<Result>;

public class ResetPasswordCommandHandler : BaseHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService identityService;

    public ResetPasswordCommandHandler(
        IIdentityService identityService,
        ILogger<ResetPasswordCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result> HandleSafe(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await this.identityService.ResetPassword(request.Email, request.Code, request.Password);
    }
}
