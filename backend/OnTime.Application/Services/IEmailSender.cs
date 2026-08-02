namespace OnTime.Application.Services;

public interface IEmailSender
{
    Task SendConfirmationEmail(string toEmail, string userId, string token, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmail(string toEmail, string token, CancellationToken cancellationToken = default);
}