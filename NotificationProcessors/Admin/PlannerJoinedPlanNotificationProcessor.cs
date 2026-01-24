using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class PlannerJoinedPlanNotificationProcessor(
    ILogger<PlannerJoinedPlanNotificationProcessor> logger,
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
        if (recipients.Count == 0) return null;

        var joinedVolunteer = await clientService.GetRecipientInfoAsync(eventData.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Planner Joined",
            $"{joinedVolunteer.Volunteer.FirstName} {joinedVolunteer.Volunteer.LastName} has joined the shift plan '{eventData.ShiftPlan.Name}'.",
            date,
            $@"/plans/{eventData.ShiftPlan.Id}",
            false,
            null
            );
    }

    public async Task<EmailNotification?> BuildEmail(ShiftPlanVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.ADMIN_PLANNER_JOINED_PLAN,
            RelatedShiftPlanId = eventData.ShiftPlan.Id,
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.ADMIN
        });
        if (recipients.Count == 0) return null;

        var joinedVolunteer = await clientService.GetRecipientInfoAsync(eventData.VolunteerId);
        var relatedPlan = await (await clientService.GetClient()).GetShiftDetailsAsync(eventData.ShiftPlan.Id);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            $"{eventData.ShiftPlan.Name}: New Planner Joined",
            $"{joinedVolunteer.Volunteer.FirstName} {joinedVolunteer.Volunteer.LastName} has joined the shift plan '{eventData.ShiftPlan.Name}' in the event '{relatedPlan.Event.Name}'." +
            $"You can manage the shift plan here: https://frontend.shiftcontrol.tobeh.host/plans/{eventData.ShiftPlan.Id}"
        );
    }
}
