using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class EventBulkRemoveNotificationProcessor(
    ILogger<EventBulkRemoveNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<UserEventBulkEvent>
{
    public Task<PushNotification?> BuildPush(UserEventBulkEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(UserEventBulkEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
