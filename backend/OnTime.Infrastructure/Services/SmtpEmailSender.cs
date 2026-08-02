using System.Net;
using System.Net.Mail;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OnTime.Application.Services;
using OnTime.Domain.Settings;

namespace OnTime.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings emailSettings;
    private readonly AuthenticationSettings authenticationSettings;
    private readonly ILogger<SmtpEmailSender> logger;

    public SmtpEmailSender(
        IOptions<EmailSettings> emailOptions,
        IOptions<AuthenticationSettings> authenticationOptions,
        ILogger<SmtpEmailSender> logger)
    {
        this.emailSettings = emailOptions.Value;
        this.authenticationSettings = authenticationOptions.Value;
        this.logger = logger;
    }

    public async Task SendEmail(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new SmtpClient(this.emailSettings.SmtpServer, this.emailSettings.SmtpPort);
            using var message = new MailMessage
            {
                From = new MailAddress(this.emailSettings.SenderEmail, this.emailSettings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message, cancellationToken);
            this.logger.LogInformation("Email enviado com sucesso para {To} com assunto: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Erro ao enviar email para {To}", to);
            throw;
        }
    }

    public async Task SendConfirmationEmail(string toEmail, string userId, string token, CancellationToken cancellationToken = default)
    {
        var clientUrl = string.IsNullOrWhiteSpace(this.authenticationSettings.ClientUrl)
            ? "http://localhost:3001"
            : this.authenticationSettings.ClientUrl.TrimEnd('/');

        var confirmationLink = $"{clientUrl}/confirm-email?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";

        var subject = "Confirme o seu endereço de email - OnTime";
        var htmlBody = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; rounded-radius: 8px;">
                <h2 style="color: #333333;">Bem-vindo à OnTime!</h2>
                <p>Obrigado por se registar na nossa plataforma. Para ativar a sua conta, por favor confirme o seu endereço de email clicando no botão abaixo:</p>
                <div style="text-align: center; margin: 30px 0;">
                    <a href="{confirmationLink}" style="background-color: #2563eb; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold; display: inline-block;">Confirmar o Meu Email</a>
                </div>
                <p style="color: #666666; font-size: 14px;">Se não conseguir clicar no botão, copie e cole o seguinte link no seu navegador:</p>
                <p style="color: #2563eb; font-size: 13px; word-break: break-all;">{confirmationLink}</p>
                <hr style="border: none; border-top: 1px solid #eeeeee; margin: 20px 0;" />
                <p style="color: #999999; font-size: 12px;">Se não criou uma conta na OnTime, por favor ignore este email.</p>
            </div>
            """;

        await SendEmail(toEmail, subject, htmlBody, cancellationToken);
    }
}