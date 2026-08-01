using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<Result>;

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
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return Result.Failure(new Error("Auth.InvalidResetParameters", "Email, token e nova palavra-passe são obrigatórios."));
        }

        var result = await this.identityService.ResetPassword(request.Email, request.Token, request.NewPassword, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure(new Error("Auth.ResetPasswordFailed", result.ErrorMessage ?? "Falha ao redefinir a palavra-passe."));
        }

        return Result.Success();
    }
}
