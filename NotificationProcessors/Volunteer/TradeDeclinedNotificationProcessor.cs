using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.NotificationProcessors.Volunteer;

public class TradeDeclinedNotificationProcessor(
    ILogger<TradeDeclinedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService,
    AppLinkService appLinkService
) : INotificationProcessor<TradeEvent>
{
    public async Task<PushNotification?> BuildPush(TradeEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_TRADE_OR_AUCTION,
            RelatedVolunteerIds = {eventData.Trade.OfferingAssignment.VolunteerId},
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.Trade.RequestedAssignment.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Trade Declined",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} declined your trade request for slot '{eventData.Trade.RequestedAssignment.PositionSlot.PositionSlotName}'!",
            date,
            appLinkService.BuildVolunteerDashboardPageUrl(eventData.Trade.OfferingAssignment.PositionSlot.ShiftPlanRefPart.EventRefPart.Id.ToString())
            );
    }

    public async Task<EmailNotification?> BuildEmail(TradeEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_TRADE_OR_AUCTION,
            RelatedVolunteerIds = {eventData.Trade.OfferingAssignment.VolunteerId},
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.Trade.RequestedAssignment.VolunteerId);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Trade Declined",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} declined your trade request for slot '{eventData.Trade.RequestedAssignment.PositionSlot.PositionSlotName}'!"
            );
    }
}
