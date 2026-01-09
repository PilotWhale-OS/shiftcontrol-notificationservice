using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Classes.Dto;
using NotificationService.Service;

namespace NotificationService.Hubs.Implementation;

public class PushNotificationHub(
    ILogger<PushNotificationHub> logger,
    PushNotificationService pushNotificationService
    ) : Hub<IPushNotificationHubReceiver>, IPushNotificationHub
{
    [Authorize]
    public async Task<ICollection<PushNotificationDto>> GetHistory()
    {
        logger.LogInformation("GetHistory called by user: {UserIdentifier}", Context.UserIdentifier);

        var userId = Context.UserIdentifier;
        if (userId is null) throw new HubException("User is not authenticated.");

        var history = await pushNotificationService.GetUserHistory(userId);
        return history.Select(n => new PushNotificationDto(n.Title, n.Notification, n.Time, n.Url, n.IsRead, n.Id)).ToList();
    }

    [Authorize]
    public async Task MarkAllAsRead()
    {
        logger.LogInformation("MarkAllAsRead called by user: {UserIdentifier}", Context.UserIdentifier);

        var userId = Context.UserIdentifier;
        if (userId is null) throw new HubException("User is not authenticated.");

        await pushNotificationService.MarkAllAsRead(userId);
    }

    [Authorize]
    public async Task ClearNotification(Guid notificationId)
    {
        logger.LogInformation("ClearNotification called by user: {UserIdentifier}, NotificationId: {NotificationId}", Context.UserIdentifier, notificationId);

        var userId = Context.UserIdentifier;
        if (userId is null) throw new HubException("User is not authenticated.");

        await pushNotificationService.ClearNotification(userId, notificationId);
    }

    [Authorize]
    public async Task ClearHistory()
    {
        logger.LogInformation("ClearHistory called by user: {UserIdentifier}", Context.UserIdentifier);

        var userId = Context.UserIdentifier;
        if (userId is null) throw new HubException("User is not authenticated.");

        await pushNotificationService.ClearHistory(userId);
    }
}
