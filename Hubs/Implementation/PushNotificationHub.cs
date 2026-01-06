using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs.Implementation;

public class PushNotificationHub(
    ILogger<PushNotificationHub> logger
    ) : Hub<IPushNotificationHubReceiver>, IPushNotificationHub
{
    [Authorize]
    public async Task<ICollection<string>> GetPendingNotifications()
    {
        logger.LogInformation("GetPendingNotifications called by user: {UserIdentifier}", Context.UserIdentifier);

        return ["hello", "world"];
    }
}
