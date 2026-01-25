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
            RelatedShiftPlanId = eventData.PositionSlot.ShiftPlanRefPart.Id.ToString(),
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
            getUrl(eventData),
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
            RelatedShiftPlanId = eventData.PositionSlot.ShiftPlanRefPart.Id.ToString(),
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.PLANNER
        });
        if (recipients.Count == 0) return null;

        var volunteer = await clientService.GetRecipientInfoAsync(eventData.VolunteerId);

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Signup Request",
            $"{volunteer.Volunteer.FirstName} {volunteer.Volunteer.LastName} wants to join slot '{eventData.PositionSlot.PositionSlotName}'!"
            );    
    }
    
    private string getUrl(PositionSlotVolunteerEvent eventData)
    {
        return $@"/events/{eventData.PositionSlot.ShiftPlanRefPart.EventRefPart.Id}/plans?planId={eventData.PositionSlot.ShiftPlanRefPart.Id}&mode=assignments&status=REQUEST_FOR_ASSIGNMENT";
    }
}
