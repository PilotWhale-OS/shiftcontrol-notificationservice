using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class UserUnlockNotificationProcessor(
    ILogger<UserUnlockNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<UserEvent>
{
    public async Task<PushNotification?> BuildPush(UserEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_STATUS_CHANGED,
            RelatedVolunteerIds = {eventData.Volunteer.Id}, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;


        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Unlocked",
            $"Your account has been unlocked!",
            date,
            getUrl(eventData)
            );    
    }

    public async Task<EmailNotification?> BuildEmail(UserEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_STATUS_CHANGED,
            RelatedVolunteerIds = {eventData.Volunteer.Id}, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;


        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Unlocked",
            $"Your account has been unlocked!"
            );    
    }
    
    private string getUrl(UserEvent eventData)
    {
        return $@"/events/{eventData.ShiftPlanRefParts[0].EventRefPart.Id}/volunteer";
    }
}
