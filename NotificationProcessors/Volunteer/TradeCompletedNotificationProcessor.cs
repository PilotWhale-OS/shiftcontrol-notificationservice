using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class TradeCompletedNotificationProcessor(
    ILogger<TradeCompletedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<AssignmentSwitchEvent>
{
    public Task<PushNotification?> BuildPush(AssignmentSwitchEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(AssignmentSwitchEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
