using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OnTime.Application.Common.Extensions;
using OnTime.Application.Domain.Results;
using OnTime.Application.Features.UserProfile.Responses;
using OnTime.Application.Services;
using OnTime.Domain.Enums;

namespace OnTime.Application.Features.UserProfile.Commands;

public record AssignProfessionalCommand(string UserId) : IRequest<Result<UserProfileResponse>>;

public class AssignProfessionalCommandHandler : BaseHandler<AssignProfessionalCommand, Result<UserProfileResponse>>
{
    private readonly IAppDbContext dbContext;

    public AssignProfessionalCommandHandler(
        IAppDbContext dbContext,
        ILogger<AssignProfessionalCommandHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<Result<UserProfileResponse>> HandleSafe(AssignProfessionalCommand request, CancellationToken cancellationToken)
    {
        var profile = await this.dbContext.UserProfiles
            .Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result<UserProfileResponse>.Failure(new Error("ProfileNotFound", "Perfil de utilizador não encontrado."));
        }

        profile.Role = UserRole.Professional;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        var response = new UserProfileResponse
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            ProfilePictureUrl = profile.ProfilePicture.BuildImageUrl(),
            Role = profile.Role
        };

        return Result<UserProfileResponse>.Success(response);
    }
}