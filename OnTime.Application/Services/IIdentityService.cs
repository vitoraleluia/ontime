using System.Threading.Tasks;

using OnTime.Application.Domain;
using OnTime.Application.Domain.Results;

namespace OnTime.Application.Services;

public interface IIdentityService
{
    Task<Result> Register(string email, string password, string firstName, string lastName, string? phoneNumber);
    Task<Result<LoginStatus>> Login(string email, string password);
    Task<Result> Logout();
    Task<Result<string>> ConfirmEmail(string userId, string code);
    Task<Result> ForgotPassword(string email);
    Task<Result> ResetPassword(string email, string code, string password);
    Task<Result> ResendEmailConfirmation(string email);
    Task<Result<string>> ProcessGoogleCallback(string? returnUrl, string? remoteError);
}