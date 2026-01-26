using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class RequestLeaveAcceptedNotificationProcessor(
    ILogger<RequestLeaveAcceptedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
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
            getUrl(eventData),
            false,
            null
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


        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Leave Request Accepted",
            $"Your request to leave slot '{eventData.PositionSlot.PositionSlotName}' was accepted!"
            );    
    }
    
    private string getUrl(PositionSlotVolunteerEvent eventData)
    {
        return $@"/events/{eventData.PositionSlot.ShiftPlanRefPart.EventRefPart.Id}/volunteer";
    }
}
