using Microsoft.AspNetCore.SignalR;
using NotificationService.Classes;
using NotificationService.Classes.Dto;
using NotificationService.Hubs;
using NotificationService.Hubs.Implementation;

namespace NotificationService.Service;

public class PushNotificationService(
    ILogger<PushNotificationService> logger,
    IHubContext<PushNotificationHub, INotificationHubReceiver> notificationHub
    )
{
    public async Task SendPushNotification(PushNotification notification)
    {
        logger.LogInformation("Sending push notification to users: {UserIds}",  notification.Recipients is null ? "everyone" : string.Join(", ", notification.Recipients));

        var notificationEvent = new PushNotificationEventDto(notification.Title, notification.Notification, notification.Url);

        if (notification.Recipients is { } recipients)
        {
            await notificationHub.Clients.Users(recipients).PushNotificationReceived(notificationEvent);
        }
        else
        {
            await notificationHub.Clients.All.PushNotificationReceived(notificationEvent);
        }
    }
}
