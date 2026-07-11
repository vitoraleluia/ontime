using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OnTime.Application.Services;
using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;

namespace OnTime.Application.Features.Auth.Commands;

public record GoogleCallbackCommand(string? ReturnUrl, string? RemoteError) : IRequest<Result<string>>;

public class GoogleCallbackCommandHandler : BaseHandler<GoogleCallbackCommand, Result<string>>
{
    private readonly IIdentityService identityService;

    public GoogleCallbackCommandHandler(
        IIdentityService identityService,
        ILogger<GoogleCallbackCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result<string>> HandleSafe(GoogleCallbackCommand request, CancellationToken cancellationToken)
    {
        return await this.identityService.ProcessGoogleCallback(request.ReturnUrl, request.RemoteError);
    }
}
