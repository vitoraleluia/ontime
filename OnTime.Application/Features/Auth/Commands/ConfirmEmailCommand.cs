using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record ConfirmEmailCommand(string UserId, string Code) : IRequest<Result<string>>;

public class ConfirmEmailCommandHandler : BaseHandler<ConfirmEmailCommand, Result<string>>
{
    private readonly IIdentityService identityService;

    public ConfirmEmailCommandHandler(
        IIdentityService identityService,
        ILogger<ConfirmEmailCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result<string>> HandleSafe(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await this.identityService.ConfirmEmail(request.UserId, request.Code);
    }
}