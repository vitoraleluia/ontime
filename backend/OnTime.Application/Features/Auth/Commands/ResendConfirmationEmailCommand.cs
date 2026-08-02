using Hangfire;

using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;
using OnTime.Domain.Enums;

namespace OnTime.Application.Features.Auth.Commands;

public record ResendConfirmationEmailCommand(string Email) : IRequest<Result>;

public class ResendConfirmationEmailCommandHandler : BaseHandler<ResendConfirmationEmailCommand, Result>
{
    private readonly IIdentityService identityService;
    private readonly IBackgroundJobClient backgroundJobClient;

    public ResendConfirmationEmailCommandHandler(
        IIdentityService identityService,
        IBackgroundJobClient backgroundJobClient,
        ILogger<ResendConfirmationEmailCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
        this.backgroundJobClient = backgroundJobClient;
    }

    protected override async Task<Result> HandleSafe(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure(new Error(ErrorCode.EmailRequired, "O endereço de email é obrigatório."));
        }

        var identityResult = await this.identityService.GenerateEmailConfirmationToken(request.Email, cancellationToken);
        if (identityResult.IsFailure)
        {
            var errorMessage = identityResult.ErrorMessage ?? "Falha ao gerar código de confirmação de email.";
            return Result.Failure(new Error(ErrorCode.ResendConfirmationFailed, errorMessage));
        }

        if (identityResult.IsSuccess && !string.IsNullOrEmpty(identityResult.Token) && !string.IsNullOrEmpty(identityResult.UserId))
        {
            this.backgroundJobClient.Enqueue<IEmailSender>(sender =>
                sender.SendConfirmationEmail(request.Email, identityResult.UserId, identityResult.Token, CancellationToken.None));
        }

        return Result.Success();
    }
}