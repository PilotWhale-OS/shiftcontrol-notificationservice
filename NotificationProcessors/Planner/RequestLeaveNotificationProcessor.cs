using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class RequestLeaveNotificationProcessor(
    ILogger<RequestLeaveNotificationProcessor> logger,
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
            "Leave Request",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} wants to leave slot '{eventData.PositionSlot.PositionSlotName}'!",
            date,
            $@"/events/TODO_INSERT_EVENT_ID/plans?planId=TODO_INSERT_PLAN_ID&mode=assignments&status=AUCTION_REQUEST_FOR_UNASSIGN",
            false,
            null
            );    
    }

    public async Task<EmailNotification?> BuildEmail(PositionSlotVolunteerEvent eventData)
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

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Leave Request",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} wants to leave slot '{eventData.PositionSlot.PositionSlotName}'!"
            );    
    }
}
