using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.NotificationProcessors.Volunteer;

public class RequestLeaveAcceptedNotificationProcessor(
    ILogger<RequestLeaveAcceptedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService,
    AppLinkService appLinkService
) : INotificationProcessor<PositionSlotVolunteerEvent>
{
    public async Task<PushNotification?> BuildPush(PositionSlotVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_REQUEST_HANDLED,
            RelatedVolunteerIds = {eventData.VolunteerId},
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Leave Request Accepted",
            $"Your request to leave slot '{eventData.PositionSlot.PositionSlotName}' was accepted!",
            date,
            appLinkService.BuildVolunteerDashboardPageUrl(eventData.PositionSlot.ShiftPlanRefPart.EventRefPart.Id.ToString())
            );
    }

    public async Task<EmailNotification?> BuildEmail(PositionSlotVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_REQUEST_HANDLED,
            RelatedVolunteerIds = {eventData.VolunteerId},
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Leave Request Accepted",
            $"Your request to leave slot '{eventData.PositionSlot.PositionSlotName}' was accepted!"
            );
    }
}
