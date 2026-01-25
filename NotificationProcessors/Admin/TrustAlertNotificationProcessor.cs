using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class TrustAlertNotificationProcessor(
    ILogger<TrustAlertNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<TrustAlertEvent>
{
    public async Task<PushNotification?> BuildPush(TrustAlertEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.ADMIN_TRUST_ALERT_RECEIVED,
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.ADMIN
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.TrustAlertPart.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Trust Alert",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName}'s behavior should be reviewed.",
            date,
            $@"TODO_INSERT_TRUST_ALERT_URL",
            false,
            null
            );
    }

    public async Task<EmailNotification?> BuildEmail(TrustAlertEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.ADMIN_TRUST_ALERT_RECEIVED,
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.ADMIN
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.TrustAlertPart.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Trust Alert",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName}'s behavior should be reviewed."
            );
    }
}
