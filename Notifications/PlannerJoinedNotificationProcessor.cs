using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class PlannerJoinedNotificationProcessor(
    ILogger<PlannerJoinedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<ShiftPlanVolunteerEvent>
{
    public async Task<PushNotification?> BuildPush(ShiftPlanVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.ADMIN_PLANNER_JOINED_PLAN,
            RelatedShiftPlanId = eventData.ShiftPlan.Id,
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.ADMIN
        });

        var joinedVolunteer = await clientService.GetRecipientInfoAsync(eventData.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Planner Joined",
            $"{joinedVolunteer.Volunteer.FistName} {joinedVolunteer.Volunteer.LastName} has joined the shift plan '{eventData.ShiftPlan.Name}'.",
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
