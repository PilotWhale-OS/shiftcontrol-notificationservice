using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class PlanBulkAddNotificationProcessor(
    ILogger<PlanBulkAddNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<UserPlanBulkEvent>
{
    public async Task<PushNotification?> BuildPush(UserPlanBulkEvent eventData)
    {
        var volunteerIds = eventData.Volunteers
            .Select(v => v.Id)
            .ToList();

        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_ROLES_CHANGED,
            RelatedVolunteerIds = volunteerIds, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;


        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "New Roles",
            $"You were assigned new roles!",
            date,
            $@"/events/TODO_INSERT_EVENT_ID",
            false,
            null
            );    
    }

    public Task<EmailNotification?> BuildEmail(UserPlanBulkEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
