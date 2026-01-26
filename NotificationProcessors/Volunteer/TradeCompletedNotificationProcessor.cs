using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class TradeCompletedNotificationProcessor(
    ILogger<TradeCompletedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<AssignmentSwitchEvent>
{
    public async Task<PushNotification?> BuildPush(AssignmentSwitchEvent eventData)
    {
        // eventData includes the trade BEFORE it was accepted
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_TRADE_OR_AUCTION,
            RelatedVolunteerIds = [eventData.OfferingAssignment.VolunteerId], 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.RequestedAssignment.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Trade Accepted",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} accepted your trade for slot '{eventData.RequestedAssignment.PositionSlot.PositionSlotName}'!",
            date,
            getUrl(eventData)
            );    
    }

    public async Task<EmailNotification?> BuildEmail(AssignmentSwitchEvent eventData)
    {
        // eventData includes the trade BEFORE it was accepted
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_TRADE_OR_AUCTION,
            RelatedVolunteerIds = [eventData.OfferingAssignment.VolunteerId], 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.RequestedAssignment.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Trade Accepted",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} accepted your trade for slot '{eventData.RequestedAssignment.PositionSlot.PositionSlotName}'!"
            );    
    }
    
    private string getUrl(AssignmentSwitchEvent eventData)
    {
        return $@"/events/{eventData.OfferingAssignment.PositionSlot.ShiftPlanRefPart.EventRefPart.Id}/volunteer";
    }
}
