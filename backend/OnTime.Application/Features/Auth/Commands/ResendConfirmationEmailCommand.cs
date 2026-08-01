using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record ResendConfirmationEmailCommand(string Email) : IRequest<Result>;

public class ResendConfirmationEmailCommandHandler : BaseHandler<ResendConfirmationEmailCommand, Result>
{
    private readonly IIdentityService identityService;

    public ResendConfirmationEmailCommandHandler(
        IIdentityService identityService,
        ILogger<ResendConfirmationEmailCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result> HandleSafe(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure(new Error("Auth.EmailRequired", "O endereço de email é obrigatório."));
        }

        var identityResult = await this.identityService.GenerateEmailConfirmationToken(request.Email, cancellationToken);
        if (identityResult.IsFailure)
        {
            var errorMessage = identityResult.ErrorMessage ?? "Falha ao gerar código de confirmação de email.";
            return Result.Failure(new Error("Auth.ResendConfirmationFailed", errorMessage));
        }

        return Result.Success();
    }
}
