using OnTime.API.Models.Domain;

namespace OnTime.API.Services.Authentication;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<RefreshToken?> ValidateAndRotateAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string userId, CancellationToken cancellationToken = default);
}
