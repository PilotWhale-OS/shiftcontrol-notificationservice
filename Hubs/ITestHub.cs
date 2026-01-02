using NotificationService.Classes.Dto;
using TypedSignalR.Client;

namespace NotificationService.Hubs;

[Hub]
public interface ITestHub
{
    public Task<TestEvent> SendTestEvent(TestEvent testEvent);
}

[Receiver]
public interface ITestHubReceiver
{
    public Task TestEventReceived(TestEvent testEvent);
}
