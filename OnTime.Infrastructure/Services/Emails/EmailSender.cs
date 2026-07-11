using System.Net.Mail;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OnTime.Application.Common.Interfaces;
using OnTime.Application.Common.Models;
using OnTime.Domain.Entities;

namespace OnTime.Infrastructure.Services.Emails;

public class EmailSender : IEmailSender<ApplicationUser>
{
    private readonly EmailSettings emailSettings;
    private readonly IServerSideRenderer renderer;
    private readonly ILogger<EmailSender> logger;

    public EmailSender(
        IOptions<EmailSettings> emailSettings,
        IServerSideRenderer renderer,
        ILogger<EmailSender> logger)
    {
        this.emailSettings = emailSettings.Value;
        this.renderer = renderer;
        this.logger = logger;
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var subject = "Confirme o seu endereço de e-mail";
        var body = await this.renderer.RenderView("~/Views/Email/ConfirmEmail.cshtml", confirmationLink);
        await SendEmail(email, subject, body);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var subject = "Recuperação de Palavra-passe";
        var body = await this.renderer.RenderView("~/Views/Email/ResetPasswordLink.cshtml", resetLink);
        await SendEmail(email, subject, body);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var subject = "Código de Recuperação de Palavra-passe";
        var body = await this.renderer.RenderView("~/Views/Email/ResetPasswordCode.cshtml", resetCode);
        await SendEmail(email, subject, body);
    }

    private async Task SendEmail(string email, string subject, string htmlMessage)
    {
        try
        {
            using var client = new SmtpClient(this.emailSettings.SmtpServer, this.emailSettings.SmtpPort);
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(this.emailSettings.SenderEmail, this.emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
            this.logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to send email to {Email}", email);
        }
    }
}
