using Hangfire;

using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

public class ForgotPasswordCommandHandler : BaseHandler<ForgotPasswordCommand, Result>
{
    private readonly IIdentityService identityService;
    private readonly IBackgroundJobClient backgroundJobClient;

    public ForgotPasswordCommandHandler(
        IIdentityService identityService,
        IBackgroundJobClient backgroundJobClient,
        ILogger<ForgotPasswordCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
        this.backgroundJobClient = backgroundJobClient;
    }

    protected override async Task<Result> HandleSafe(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure(new Error("Auth.EmailRequired", "O endereço de email é obrigatório."));
        }

        var result = await this.identityService.GeneratePasswordResetToken(request.Email, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure(new Error("Auth.ForgotPasswordFailed", result.ErrorMessage ?? "Falha ao gerar pedido de recuperação de palavra-passe."));
        }

        if (result.IsSuccess && !string.IsNullOrEmpty(result.Token))
        {
            this.backgroundJobClient.Enqueue<IEmailSender>(sender =>
                sender.SendPasswordResetEmail(request.Email, result.Token, CancellationToken.None));
        }

        // Return success regardless of whether email exists to prevent user enumeration
        return Result.Success();
    }
}