using System.Diagnostics;
using NotificationService.Classes.Dto;
using TypedSignalR.Client;

namespace NotificationService.Hubs;

[Hub]
public interface IPushNotificationHub
{
    public Task<ICollection<PushNotificationEventDto>> GetPendingNotifications();
}

[Receiver]
public interface IPushNotificationHubReceiver
{
    public Task PushNotificationReceived(PushNotificationEventDto notificationEvent);
}
