using System.Text.RegularExpressions;

using Hangfire;

using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;
using UserProfileEntity = OnTime.Domain.Entities.UserProfile;

namespace OnTime.Application.Features.Auth.Commands;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber) : IRequest<Result<string>>;

public class RegisterUserCommandHandler : BaseHandler<RegisterUserCommand, Result<string>>
{
    private static readonly Regex PtPhoneRegex = new(@"^(\+351)?9\d{8}$", RegexOptions.Compiled);
    private readonly IIdentityService identityService;
    private readonly IAppDbContext dbContext;
    private readonly IBackgroundJobClient backgroundJobClient;

    public RegisterUserCommandHandler(
        IIdentityService identityService,
        IAppDbContext dbContext,
        IBackgroundJobClient backgroundJobClient,
        ILogger<RegisterUserCommandHandler> logger) : base(logger)
    {
        this.identityService = identityService;
        this.dbContext = dbContext;
        this.backgroundJobClient = backgroundJobClient;
    }

    protected override async Task<Result<string>> HandleSafe(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return Result<string>.Failure(new Error("Auth.FirstNameRequired", "O primeiro nome é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            return Result<string>.Failure(new Error("Auth.LastNameRequired", "O apelido é obrigatório."));
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && !PtPhoneRegex.IsMatch(request.PhoneNumber))
        {
            return Result<string>.Failure(new Error("Auth.InvalidPhoneNumber", "O número de telemóvel deve ser um número português válido (ex: 927431783)."));
        }

        var identityResult = await this.identityService.RegisterUser(request.Email, request.Password, cancellationToken);
        if (identityResult.IsFailure)
        {
            var errorMessage = identityResult.ErrorMessage ?? "Falha ao criar conta de utilizador.";
            return Result<string>.Failure(new Error("Auth.RegistrationFailed", errorMessage));
        }

        var profile = new UserProfileEntity
        {
            Id = identityResult.UserId!,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        this.dbContext.UserProfiles.Add(profile);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        // Generate email confirmation token and enqueue Hangfire job
        var tokenResult = await this.identityService.GenerateEmailConfirmationToken(request.Email, cancellationToken);
        if (tokenResult.IsSuccess && !string.IsNullOrEmpty(tokenResult.Token))
        {
            this.backgroundJobClient.Enqueue<IEmailSender>(sender =>
                sender.SendConfirmationEmail(request.Email, identityResult.UserId!, tokenResult.Token, CancellationToken.None));
        }

        return Result<string>.Success(profile.Id);
    }
}
