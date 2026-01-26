using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class EventBulkRemoveNotificationProcessor(
    ILogger<EventBulkRemoveNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<UserEventBulkEvent>
{
    public async Task<PushNotification?> BuildPush(UserEventBulkEvent eventData)
    {
        var volunteerIds = eventData.Volunteers
            .Select(v => v.Id)
            .ToList();
        
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_PLANS_CHANGED,
            RelatedVolunteerIds = volunteerIds, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;


        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Shiftplan Unassignments",
            $"You were unassigned from events!",
            date,
            getUrl(),
            false,
            null
            );    
    }

    public async Task<EmailNotification?> BuildEmail(UserEventBulkEvent eventData)
    {
        var volunteerIds = eventData.Volunteers
            .Select(v => v.Id)
            .ToList();
        
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_PLANS_CHANGED,
            RelatedVolunteerIds = volunteerIds, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;


        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Shiftplan Unassignments",
            $"You were unassigned from events!"
            );    
    }
    
    private string getUrl()
    {
        return $@"/events";
    }
}
