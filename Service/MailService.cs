using System.Threading.Channels;
using NotificationService.Classes;

namespace NotificationService.Service;

public class MailService(ILogger<MailService> logger, Channel<EmailNotification> channel)
{
    public Task PublishEmail(EmailNotification email)
    {
        logger.LogDebug("Queueing email to {Recipients}", string.Join(", ", email.Recipients.Select(r => r.Email)));
        return channel.Writer.WriteAsync(email).AsTask();
    }
}
