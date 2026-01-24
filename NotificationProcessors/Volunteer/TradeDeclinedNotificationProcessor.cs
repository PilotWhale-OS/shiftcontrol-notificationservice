using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class TradeDeclinedNotificationProcessor(
    ILogger<TradeDeclinedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<TradeEvent>
{
    public Task<PushNotification?> BuildPush(TradeEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(TradeEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
