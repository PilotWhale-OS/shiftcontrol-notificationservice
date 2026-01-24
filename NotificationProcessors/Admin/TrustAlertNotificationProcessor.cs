using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class TrustAlertNotificationProcessor(
    ILogger<TrustAlertNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<TrustAlertEvent>
{
    public Task<PushNotification?> BuildPush(TrustAlertEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(TrustAlertEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
