using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class AuctionClaimedNotificationProcessor(
    ILogger<AuctionClaimedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<AssignmentEvent>
{
    public async Task<PushNotification?> BuildPush(AssignmentEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_TRADE_OR_AUCTION,
            // TODO add oldAssigned User
            RelatedVolunteerIds = {}, 
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Auction Claimed",
            $"Your auction for slot '{eventData.Assignment.PositionSlot.PositionSlotName}' has been claimed!",
            date,
            $@"/events/TODO_INSERT_EVENT_ID/volunteer",
            false,
            null
            );    
    }

    public Task<EmailNotification?> BuildEmail(AssignmentEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
