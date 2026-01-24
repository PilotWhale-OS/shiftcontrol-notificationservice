using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class RoleUnassignedNotificationProcessor(
    ILogger<RoleUnassignedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<RoleVolunteerEvent>
{
    public async Task<PushNotification?> BuildPush(RoleVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_ROLES_CHANGED,
            RelatedVolunteerIds = {eventData.VolunteerId}, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;


        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Role Unassignment",
            $"The role '{eventData.Role.Name}' has been unassigned from you!",
            date,
            $@"/events/TODO_INSERT_EVENT_ID",
            false,
            null
            );    
    }

    public Task<EmailNotification?> BuildEmail(RoleVolunteerEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
