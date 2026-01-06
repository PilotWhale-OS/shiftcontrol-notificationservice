using System.Diagnostics;
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
