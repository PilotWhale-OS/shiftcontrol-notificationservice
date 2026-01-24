using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class RequestJoinAcceptedNotificationProcessor(
    ILogger<RequestJoinAcceptedNotificationProcessor> logger,
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
            "Join Request Accepted",
            $"Your request to join slot '{eventData.PositionSlot.PositionSlotName}' was accepted!",
            date,
            $@"/events/TODO_INSERT_EVENT_ID/volunteer",
            false,
            null
            );    
    }

    public Task<EmailNotification?> BuildEmail(PositionSlotVolunteerEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
