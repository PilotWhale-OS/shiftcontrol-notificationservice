using NotificationService.Classes.Dto;
using TypedSignalR.Client;

namespace NotificationService.Hubs;

[Hub]
public interface ITestHub
{
    public Task<TestEventDto> SendTestEvent(TestEventDto testEventDto);
}

[Receiver]
public interface ITestHubReceiver
{
    public Task TestEventReceived(TestEventDto testEventDto);
}
