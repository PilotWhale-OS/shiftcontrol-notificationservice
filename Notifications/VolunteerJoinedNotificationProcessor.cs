using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class VolunteerJoinedNotificationProcessor(
    ILogger<VolunteerJoinedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<ShiftPlanVolunteerEvent>
{
    public async Task<PushNotification?> BuildPush(ShiftPlanVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.PLANNER_VOLUNTEER_JOINED_PLAN,
            RelatedShiftPlanId = eventData.ShiftPlan.Id,
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.PLANNER
        });

        var joinedVolunteer = await clientService.GetRecipientInfoAsync(eventData.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(r => r.Id).ToList(),
            "Volunteer Joined",
            $"{joinedVolunteer.FistName} {joinedVolunteer.LastName} has joined the shift plan '{eventData.ShiftPlan.Name}'.",
            date,
            $@"/plans/{eventData.ShiftPlan.Id}",
            false,
            null
            );
    }

    public Task<EmailNotification?> BuildEmail(ShiftPlanVolunteerEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
