using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class VolunteerJoinedPlanNotificationProcessor(
    ILogger<VolunteerJoinedPlanNotificationProcessor> logger,
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
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Volunteer Joined",
            $"{joinedVolunteer.Volunteer.FirstName} {joinedVolunteer.Volunteer.LastName} has joined the shift plan '{eventData.ShiftPlan.Name}'.",
            date,
            $@"/events/TODO_INSERT_EVENT_ID/plans/{eventData.ShiftPlan.Id}",
            false,
            null
            );
    }

    public Task<EmailNotification?> BuildEmail(ShiftPlanVolunteerEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
