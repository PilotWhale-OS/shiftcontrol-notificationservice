using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class AuctionClaimedNotificationProcessor(
    ILogger<AuctionClaimedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<AssignmentEvent>
{
    public Task<PushNotification?> BuildPush(AssignmentEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(AssignmentEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
