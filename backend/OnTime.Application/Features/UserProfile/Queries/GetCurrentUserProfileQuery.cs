using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OnTime.Application.Common.Extensions;
using OnTime.Application.Domain.Results;
using OnTime.Application.Features.UserProfile.Responses;
using OnTime.Application.Services;
using OnTime.Domain.Enums;

namespace OnTime.Application.Features.UserProfile.Queries;

public record GetCurrentUserProfileQuery(
    string UserId,
    string Email,
    string FirstName,
    string LastName) : IRequest<Result<UserProfileResponse>>;

public class GetCurrentUserProfileQueryHandler : BaseHandler<GetCurrentUserProfileQuery, Result<UserProfileResponse>>
{
    private readonly IAppDbContext dbContext;

    public GetCurrentUserProfileQueryHandler(
        IAppDbContext dbContext,
        ILogger<GetCurrentUserProfileQueryHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<Result<UserProfileResponse>> HandleSafe(GetCurrentUserProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await this.dbContext.UserProfiles
            .Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (profile == null)
        {
            // Just-In-Time Profile creation for new Keycloak registrations
            profile = new OnTime.Domain.Entities.UserProfile
            {
                Id = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = UserRole.Client
            };

            this.dbContext.UserProfiles.Add(profile);
            await this.dbContext.SaveChangesAsync(cancellationToken);
        }

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