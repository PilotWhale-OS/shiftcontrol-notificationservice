using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.NotificationProcessors.Planner;

public class RequestLeaveNotificationProcessor(
    ILogger<RequestLeaveNotificationProcessor> logger,
    ShiftserviceApiClientService clientService,
    AppLinkService appLinkService
) : INotificationProcessor<PositionSlotVolunteerEvent>
{
    public async Task<PushNotification?> BuildPush(PositionSlotVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.PLANNER_VOLUNTEER_REQUEST,
            RelatedShiftPlanId = eventData.PositionSlot.ShiftPlanRefPart.Id.ToString(),
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.PLANNER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Leave Request",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} wants to leave slot '{eventData.PositionSlot.PositionSlotName}'!",
            date,
            appLinkService.BuildPlanUnassignmentRequestsPageUrl(
                eventData.PositionSlot.ShiftPlanRefPart.EventRefPart.Id.ToString(),
                eventData.PositionSlot.PositionSlotId.ToString()
            ));
    }

    public async Task<EmailNotification?> BuildEmail(PositionSlotVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.PLANNER_VOLUNTEER_REQUEST,
            RelatedShiftPlanId = eventData.PositionSlot.ShiftPlanRefPart.Id.ToString(),
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.PLANNER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.VolunteerId);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Leave Request",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} wants to leave slot '{eventData.PositionSlot.PositionSlotName}'!"
            );
    }
}
