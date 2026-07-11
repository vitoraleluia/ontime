using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record LogoutCommand() : IRequest<Result>;

public class LogoutCommandHandler : BaseHandler<LogoutCommand, Result>
{
    private readonly IIdentityService identityService;

    public LogoutCommandHandler(
        IIdentityService identityService,
        ILogger<LogoutCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result> HandleSafe(LogoutCommand request, CancellationToken cancellationToken)
    {
        return await this.identityService.Logout();
    }
}