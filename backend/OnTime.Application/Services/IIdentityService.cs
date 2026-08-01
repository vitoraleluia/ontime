using OnTime.Domain.Enums;

namespace OnTime.Application.Services;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, UserRole role, CancellationToken cancellationToken = default);
    Task<bool> AssignRoleAsync(string userId, UserRole role, CancellationToken cancellationToken = default);
}
