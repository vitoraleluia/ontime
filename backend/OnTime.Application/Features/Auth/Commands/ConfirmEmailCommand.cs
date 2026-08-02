using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;
using OnTime.Domain.Enums;

namespace OnTime.Application.Features.Auth.Commands;

public record ConfirmEmailCommand(
    string UserId,
    string Token) : IRequest<Result>;

public class ConfirmEmailCommandHandler : BaseHandler<ConfirmEmailCommand, Result>
{
    private readonly IIdentityService identityService;

    public ConfirmEmailCommandHandler(
        IIdentityService identityService,
        ILogger<ConfirmEmailCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result> HandleSafe(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Token))
        {
            return Result.Failure(new Error(ErrorCode.InvalidConfirmationParameters, "ID de utilizador e token são obrigatórios."));
        }

        var result = await this.identityService.ConfirmEmail(request.UserId, request.Token, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure(new Error(ErrorCode.ConfirmEmailFailed, result.ErrorMessage ?? "Falha ao confirmar o endereço de email."));
        }

        return Result.Success();
    }
}