namespace OnTime.Application.Services;

public interface IEmailSender
{
    Task SendEmail(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task SendConfirmationEmail(string toEmail, string userId, string token, CancellationToken cancellationToken = default);
}
