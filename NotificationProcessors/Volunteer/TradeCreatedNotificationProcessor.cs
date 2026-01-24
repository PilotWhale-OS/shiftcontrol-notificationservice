using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class TradeCreatedNotificationProcessor(
    ILogger<TradeCreatedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<TradeEvent>
{
    public async Task<PushNotification?> BuildPush(TradeEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_TRADE_OR_AUCTION,
            RelatedVolunteerIds = {eventData.Trade.RequestedAssignment.VolunteerId}, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.Trade.OfferingAssignment.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Trade Requested",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} offers you '{eventData.Trade.OfferingAssignment.PositionSlot.PositionSlotName}' for '{eventData.Trade.RequestedAssignment.PositionSlot.PositionSlotName}'!",
            date,
            $@"/events/TODO_INSERT_EVENT_ID/volunteer",
            false,
            null
            );    
    }

    public Task<EmailNotification?> BuildEmail(TradeEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
