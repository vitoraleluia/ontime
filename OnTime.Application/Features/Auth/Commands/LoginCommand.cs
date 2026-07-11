using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OnTime.Application.Services;
using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;

namespace OnTime.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginStatus>>;

public class LoginCommandHandler : BaseHandler<LoginCommand, Result<LoginStatus>>
{
    private readonly IIdentityService identityService;

    public LoginCommandHandler(
        IIdentityService identityService,
        ILogger<LoginCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
    }

    protected override async Task<Result<LoginStatus>> HandleSafe(LoginCommand request, CancellationToken cancellationToken)
    {
        return await this.identityService.Login(request.Email, request.Password);
    }
}
