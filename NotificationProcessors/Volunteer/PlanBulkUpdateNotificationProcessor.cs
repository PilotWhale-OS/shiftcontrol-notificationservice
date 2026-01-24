using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class PlanBulkUpdateNotificationProcessor(
    ILogger<PlanBulkUpdateNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<UserEvent>
{
    public async Task<PushNotification?> BuildPush(UserEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_ROLES_CHANGED,
            RelatedVolunteerIds = {eventData.Volunteer.Id}, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;


        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Roles Updated",
            $"Your assigned roles have changed!",
            date,
            $@"/events/TODO_INSERT_EVENT_ID/volunteer",
            false,
            null
            );    
    }

    public Task<EmailNotification?> BuildEmail(UserEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
