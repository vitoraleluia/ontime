using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record LogoutUserCommand : IRequest<Result>;

public class LogoutUserCommandHandler : BaseHandler<LogoutUserCommand, Result>
{
    private readonly IIdentityService identityService;

    public LogoutUserCommandHandler(
        IIdentityService identityService,
        ILogger<LogoutUserCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result> HandleSafe(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await this.identityService.SignOut(cancellationToken);
        return Result.Success();
    }
}
