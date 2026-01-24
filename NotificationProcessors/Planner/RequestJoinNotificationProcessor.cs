using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class RequestJoinNotificationProcessor(
    ILogger<RequestJoinNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<PositionSlotVolunteerEvent>
{
    public async Task<PushNotification?> BuildPush(PositionSlotVolunteerEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.PLANNER_VOLUNTEER_REQUEST,
            // TODO add shiftplanid
            RelatedShiftPlanId = null,
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.PLANNER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Signup Request",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} wants to join slot '{eventData.PositionSlot.PositionSlotName}'!",
            date,
            $@"/events/TODO_INSERT_EVENT_ID/plans?planId=TODO_INSERT_PLAN_ID&mode=assignments&status=REQUEST_FOR_ASSIGNMENT",
            false,
            null
            );    
    }

    public Task<EmailNotification?> BuildEmail(PositionSlotVolunteerEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
