using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Extensions;
using OnTime.Application.Features.UserProfile.Responses;
using OnTime.Application.Services;

using UserProfileEntity = OnTime.Domain.Entities.UserProfile;

namespace OnTime.Application.Features.UserProfile.Commands;

public record CreateUserProfileCommand(
    string UserId,
    string Email,
    string FirstName,
    string LastName) : IRequest<Result<UserProfileResponse>>;

public class CreateUserProfileCommandHandler : BaseHandler<CreateUserProfileCommand, Result<UserProfileResponse>>
{
    private readonly IAppDbContext dbContext;

    public CreateUserProfileCommandHandler(
        IAppDbContext dbContext,
        ILogger<CreateUserProfileCommandHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<Result<UserProfileResponse>> HandleSafe(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var existingProfile = await this.dbContext.UserProfiles
            .Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (existingProfile != null)
        {
            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                existingProfile.FirstName = request.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                existingProfile.LastName = request.LastName;
            }

            await this.dbContext.SaveChangesAsync(cancellationToken);

            var existingResponse = new UserProfileResponse
            {
                FirstName = existingProfile.FirstName,
                LastName = existingProfile.LastName,
                Email = existingProfile.Email,
                PhoneNumber = existingProfile.PhoneNumber,
                ProfilePictureUrl = existingProfile.ProfilePicture.BuildImageUrl()
            };

            return Result<UserProfileResponse>.Success(existingResponse);
        }

        var newProfile = new UserProfileEntity
        {
            Id = request.UserId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        this.dbContext.UserProfiles.Add(newProfile);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        var createdResponse = new UserProfileResponse
        {
            FirstName = newProfile.FirstName,
            LastName = newProfile.LastName,
            Email = newProfile.Email,
            PhoneNumber = newProfile.PhoneNumber,
            ProfilePictureUrl = null
        };

        return Result<UserProfileResponse>.Success(createdResponse);
    }
}