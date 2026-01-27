using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.NotificationProcessors.Admin;

public class PlannerJoinedPlanNotificationProcessor(
    ILogger<PlannerJoinedPlanNotificationProcessor> logger,
    ShiftserviceApiClientService clientService,
    AppLinkService appLinkService
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
            appLinkService.BuildVolunteerPlansPageUrl(joinedVolunteer.Volunteer.Id)
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

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            $"{eventData.ShiftPlan.Name}: New Planner Joined",
            $"{joinedVolunteer.Volunteer.FirstName} {joinedVolunteer.Volunteer.LastName} has joined the shift plan '{eventData.ShiftPlan.Name}' in the event '{eventData.ShiftPlan.EventRefPart.Name}'." +
            $"You can manage the volunteer access here: {appLinkService.BuildVolunteerPlansPageUrl(joinedVolunteer.Volunteer.Id, LinkMode.Absolute)}"
        );
    }
}
