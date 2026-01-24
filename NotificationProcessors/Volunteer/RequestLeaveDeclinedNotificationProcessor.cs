using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class RequestLeaveDeclinedNotificationProcessor(
    ILogger<RequestLeaveDeclinedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<PositionSlotVolunteerEvent>
{
    public Task<PushNotification?> BuildPush(PositionSlotVolunteerEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(PositionSlotVolunteerEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
