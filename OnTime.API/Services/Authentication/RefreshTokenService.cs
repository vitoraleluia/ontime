using System.Security.Cryptography;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using OnTime.API.Database;
using OnTime.API.Models.Domain;
using OnTime.API.Models.Settings;

namespace OnTime.API.Services.Authentication;

public class RefreshTokenService(AppDbContext db, IOptions<JwtSettings> jwtSettings) : IRefreshTokenService
{
    public async Task<RefreshToken> GenerateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Delete any existing refresh token for this user (only keep 1)
        var existingTokens = await db.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync(cancellationToken);

        if (existingTokens.Count != 0)
        {
            db.RefreshTokens.RemoveRange(existingTokens);
        }

        // Generate new refresh token
        var token = GenerateToken();
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }

    public async Task<RefreshToken?> ValidateAndRotateAsync(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        if (refreshToken == null || refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        var user = refreshToken.User;

        // Delete old token
        db.RefreshTokens.Remove(refreshToken);

        // Generate new token
        var newToken = await GenerateRefreshTokenAsync(user.Id, cancellationToken);
        return newToken;
    }

    public async Task RevokeTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var existingTokens = await db.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync(cancellationToken);

        if (existingTokens.Count != 0)
        {
            db.RefreshTokens.RemoveRange(existingTokens);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private static string GenerateToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}