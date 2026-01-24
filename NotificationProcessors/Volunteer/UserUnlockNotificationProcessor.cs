using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class UserUnlockNotificationProcessor(
    ILogger<UserUnlockNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<UserEvent>
{
    public Task<PushNotification?> BuildPush(UserEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(UserEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
