using System.Threading.Channels;
using MailKit.Net.Smtp;
using MailKit.Security;
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
        var settings = emailSettings.Value;
        if (settings.EnableSending == false)
        {
            logger.LogWarning("Email sending is disabled. Mail client will not connect to SMTP server.");
            return;
        }

        try
        {
            await _client.ConnectAsync(settings.SmtpHost, settings.SmtpPort, settings.SecureSocketOptions, cancellationToken);

            if (HasSmtpCredentials(settings))
            {
                EnsureValidSmtpCredentials(settings);
                await _client.AuthenticateAsync(settings.SmtpUsername, settings.SmtpPassword, cancellationToken);
            }
            else
            {
                logger.LogInformation("SMTP authentication is disabled because no username/password were configured.");
            }

            logger.LogInformation(
                "Connected to SMTP server at {SmtpHost}:{SmtpPort} using {SecureSocketOptions}",
                settings.SmtpHost,
                settings.SmtpPort,
                settings.SecureSocketOptions
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to SMTP server at {SmtpHost}:{SmtpPort}", settings.SmtpHost, settings.SmtpPort);
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
        if (emailSettings.Value.EnableSending == false)
        {
            logger.LogWarning("Email sending is disabled. Mail client was not connected to SMTP server.");
            return;
        }

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
        var settings = emailSettings.Value;
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(settings.FromName, settings.FromEmail));
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

    private static bool HasSmtpCredentials(EmailSettings settings)
    {
        return string.IsNullOrWhiteSpace(settings.SmtpUsername) == false
            || string.IsNullOrWhiteSpace(settings.SmtpPassword) == false;
    }

    private static void EnsureValidSmtpCredentials(EmailSettings settings)
    {
        var hasUsername = string.IsNullOrWhiteSpace(settings.SmtpUsername) == false;
        var hasPassword = string.IsNullOrWhiteSpace(settings.SmtpPassword) == false;

        if (hasUsername && hasPassword)
        {
            return;
        }

        throw new InvalidOperationException("Both Email:SmtpUsername and Email:SmtpPassword must be set when SMTP authentication is enabled.");
    }
}
