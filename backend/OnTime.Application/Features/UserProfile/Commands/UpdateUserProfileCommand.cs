using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OnTime.Application.Common.Extensions;
using OnTime.Application.Domain.Results;
using OnTime.Application.Features.UserProfile.Responses;
using OnTime.Application.Services;

namespace OnTime.Application.Features.UserProfile.Commands;

public record UpdateUserProfileCommand(
    string UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Guid? ProfilePictureId) : IRequest<Result<UserProfileResponse>>;

public class UpdateUserProfileCommandHandler : BaseHandler<UpdateUserProfileCommand, Result<UserProfileResponse>>
{
    private readonly IAppDbContext dbContext;

    public UpdateUserProfileCommandHandler(
        IAppDbContext dbContext,
        ILogger<UpdateUserProfileCommandHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<Result<UserProfileResponse>> HandleSafe(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await this.dbContext.UserProfiles
            .Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result<UserProfileResponse>.Failure(new Error("ProfileNotFound", "Perfil de utilizador não encontrado."));
        }

        profile.FirstName = request.FirstName;
        profile.LastName = request.LastName;
        profile.PhoneNumber = request.PhoneNumber;

        // Guard clause pattern for checking / assigning profile picture
        if (request.ProfilePictureId == null)
        {
            profile.ProfilePictureId = null;
        }
        else
        {
            var imageExists = await this.dbContext.Images.AnyAsync(i => i.Id == request.ProfilePictureId.Value, cancellationToken);
            if (imageExists)
            {
                profile.ProfilePictureId = request.ProfilePictureId;
            }
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        var updatedProfile = await this.dbContext.UserProfiles
            .Include(u => u.ProfilePicture)
            .FirstAsync(u => u.Id == request.UserId, cancellationToken);

        var response = new UserProfileResponse
        {
            FirstName = updatedProfile.FirstName,
            LastName = updatedProfile.LastName,
            Email = updatedProfile.Email,
            PhoneNumber = updatedProfile.PhoneNumber,
            ProfilePictureUrl = updatedProfile.ProfilePicture.BuildImageUrl(),
            Role = updatedProfile.Role
        };

        return Result<UserProfileResponse>.Success(response);
    }
}