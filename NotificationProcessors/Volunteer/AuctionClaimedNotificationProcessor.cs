using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.NotificationProcessors.Volunteer;

public class AuctionClaimedNotificationProcessor(
    ILogger<AuctionClaimedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService,
    AppLinkService appLinkService
) : INotificationProcessor<ClaimedAuctionEvent>
{
    public async Task<PushNotification?> BuildPush(ClaimedAuctionEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_TRADE_OR_AUCTION,
            RelatedVolunteerIds = {eventData.OldVolunteerId},
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Auction Claimed",
            $"Your auction for slot '{eventData.Assignment.PositionSlot.PositionSlotName}' has been claimed!",
            date,
            appLinkService.BuildVolunteerDashboardPageUrl(eventData.Assignment.PositionSlot.ShiftPlanRefPart.EventRefPart.Id.ToString())
            );
    }

    public async Task<EmailNotification?> BuildEmail(ClaimedAuctionEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_TRADE_OR_AUCTION,
            RelatedVolunteerIds = {eventData.OldVolunteerId},
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Auction Claimed",
            $"Your auction for slot '{eventData.Assignment.PositionSlot.PositionSlotName}' has been claimed!"
            );
    }
}
