using System.Threading.Channels;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Classes;
using NotificationService.Settings;

namespace NotificationService.Service;

public class MailClient(
    ILogger<MailClient> logger,
    IOptions<EmailSettings> emailSettings,
    Channel<EmailNotification> mailChannel
    ) : BackgroundService
{
    private readonly SmtpClient _client = new ();
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _client!.ConnectAsync(emailSettings.Value.SmtpHost, emailSettings.Value.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
            await _client.AuthenticateAsync(emailSettings.Value.SmtpUsername, emailSettings.Value.SmtpPassword, cancellationToken);
            logger.LogInformation("Connected to SMTP server at {SmtpHost}:{SmtpPort}", emailSettings.Value.SmtpHost, emailSettings.Value.SmtpPort);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to SMTP server at {SmtpHost}:{SmtpPort}", emailSettings.Value.SmtpHost, emailSettings.Value.SmtpPort);
            throw;
        }

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting mail client loop");
        await foreach (var message in mailChannel.Reader.ReadAllAsync(cancellationToken))
        {
            await SendMailAsync(message, cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisconnectAsync(true, cancellationToken);
        _client.Dispose();
        logger.LogInformation("Disconnected from SMTP server.");

        await base.StopAsync(cancellationToken);
    }

    private async Task SendMailAsync(EmailNotification email, CancellationToken cancellationToken)
    {
        logger.LogDebug("Preparing to send email to {Recipients}", string.Join(", ", email.Recipients.Select(r => r.Email)));

        if (emailSettings.Value.EnableSending == false)
        {
            logger.LogWarning("Email sending is disabled. Skipping sending email");
            return;
        }

        var mailMessage = BuildMail(email);
        try
        {
            await _client.SendAsync(mailMessage, cancellationToken);
            logger.LogInformation("Email sent to {Recipients}", string.Join(", ", email.Recipients.Select(r => r.Email)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending email to {Recipients}", string.Join(", ", email.Recipients.Select(r => r.Email)));

        }
    }

    private MimeMessage BuildMail(EmailNotification notification)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("ShiftControl Notifications", "noreply@shiftcontrol.com"));
        foreach (var recipient in notification.Recipients)
        {
            mailMessage.To.Add(new MailboxAddress($@"{recipient.FirstName} {recipient.LastName}", recipient.Email));
        }
        mailMessage.Subject = notification.Subject;
        mailMessage.Body = new TextPart("plain")
        {
            Text = notification.Content
        };

        return mailMessage;
    }
}
