using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;

namespace OnTime.Application.Features.Auth.Commands;

public record LoginUserCommand(
    string Email,
    string Password) : IRequest<Result>;

public class LoginUserCommandHandler : BaseHandler<LoginUserCommand, Result>
{
    private readonly IIdentityService identityService;
    private readonly IConfiguration configuration;

    public LoginUserCommandHandler(
        IIdentityService identityService,
        IConfiguration configuration,
        ILogger<LoginUserCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
        this.configuration = configuration;
    }

    protected override async Task<Result> HandleSafe(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var result = await this.identityService.PasswordSignIn(request.Email, request.Password, lockoutOnFailure: true, cancellationToken);

        if (result.IsLockedOut)
        {
            var lockoutMinutes = this.configuration.GetValue<int>("AuthenticationSettings:Lockout:DefaultLockoutTimeSpanInMinutes", 5);
            return Result.Failure(new Error("Auth.LockedOut", $"Conta bloqueada temporariamente. Tente novamente após {lockoutMinutes} minutos."));
        }

        if (result.IsFailure)
        {
            return Result.Failure(new Error("Auth.InvalidCredentials", result.ErrorMessage ?? "Credenciais inválidas. Verifique o email e a palavra-passe."));
        }

        return Result.Success();
    }
}
