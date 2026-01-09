using System.Diagnostics;
using NotificationService.Classes.Dto;
using TypedSignalR.Client;

namespace NotificationService.Hubs;

[Hub]
public interface IPushNotificationHub
{
    public Task<ICollection<PushNotificationDto>> GetHistory();
    public Task MarkAllAsRead();
    public Task ClearNotification(Guid notificationId);
    public Task ClearHistory();
}

[Receiver]
public interface IPushNotificationHubReceiver
{
    public Task PushNotificationReceived(PushNotificationDto notification);
}
