using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Extensions;
using OnTime.Application.Features.UserProfile.Responses;
using OnTime.Application.Services;
using OnTime.Domain.Enums;

namespace OnTime.Application.Features.UserProfile.Queries;

public record GetCurrentUserProfileQuery(
    string UserId) : IRequest<Result<UserProfileResponse>>;

public class GetCurrentUserProfileQueryHandler : BaseHandler<GetCurrentUserProfileQuery, Result<UserProfileResponse>>
{
    private readonly IAppDbContext dbContext;

    public GetCurrentUserProfileQueryHandler(
        IAppDbContext dbContext,
        ILogger<GetCurrentUserProfileQueryHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<Result<UserProfileResponse>> HandleSafe(GetCurrentUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var profile = await this.dbContext.UserProfiles
            .Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result<UserProfileResponse>.Failure(new Error(ErrorCode.ProfileNotFound, "Perfil de utilizador não encontrado."));
        }

        var response = new UserProfileResponse
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            ProfilePictureUrl = profile.ProfilePicture.BuildImageUrl()
        };

        return Result<UserProfileResponse>.Success(response);
    }
}