using System.Diagnostics;
using NotificationService.Classes.Dto;
using TypedSignalR.Client;

namespace NotificationService.Hubs;

[Hub]
public interface ITestHub
{
    public Task<ActivityEvent> SendActivityCreatedEvent(ActivityEvent activityEvent);
}

[Receiver]
public interface ITestHubReceiver
{
    public Task ActivityCreatedEventReceived(ActivityEvent activityEvent);
}
