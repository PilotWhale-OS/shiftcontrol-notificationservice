using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Classes.Dto;

namespace NotificationService.Hubs.Implementation;

public class PushNotificationHub(
    ILogger<PushNotificationHub> logger
    ) : Hub<IPushNotificationHubReceiver>, IPushNotificationHub
{
    [Authorize]
    public async Task<ICollection<PushNotificationEventDto>> GetPendingNotifications()
    {
        logger.LogInformation("GetPendingNotifications called by user: {UserIdentifier}", Context.UserIdentifier);

        return [
            new PushNotificationEventDto("Sample Notification", "This is a sample notification message.", DateTime.UtcNow, "http://samle.url")
        ];
    }
}
