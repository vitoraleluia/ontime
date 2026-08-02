using OnTime.Application.Models.Identity;
using OnTime.Domain.Enums;

namespace OnTime.Application.Services;

public interface IIdentityService
{
    Task<bool> IsInRole(string userId, UserRole role, CancellationToken cancellationToken = default);
    Task<bool> AssignRole(string userId, UserRole role, CancellationToken cancellationToken = default);
    Task<IdentityResult> RegisterUser(string email, string password, CancellationToken cancellationToken = default);
    Task<IdentityResult> PasswordSignIn(string email, string password, bool lockoutOnFailure = true, CancellationToken cancellationToken = default);
    Task<IdentityResult> GeneratePasswordResetToken(string email, CancellationToken cancellationToken = default);
    Task<IdentityResult> ResetPassword(string email, string token, string newPassword, CancellationToken cancellationToken = default);
    Task<IdentityResult> GenerateEmailConfirmationToken(string email, CancellationToken cancellationToken = default);
    Task<IdentityResult> ConfirmEmail(string userId, string token, CancellationToken cancellationToken = default);
    Task SignOut(CancellationToken cancellationToken = default);
}