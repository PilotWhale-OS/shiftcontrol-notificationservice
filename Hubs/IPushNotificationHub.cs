using System.Diagnostics;
using NotificationService.Classes.Dto;
using TypedSignalR.Client;

namespace NotificationService.Hubs;

[Hub]
public interface IPushNotificationHub
{
    public Task<ICollection<string>> GetPendingNotifications();
}

[Receiver]
public interface INotificationHubReceiver
{
    public Task PushNotificationReceived(PushNotificationEventDto notificationEvent);
}
