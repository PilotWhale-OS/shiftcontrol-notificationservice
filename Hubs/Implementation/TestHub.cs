using Microsoft.AspNetCore.SignalR;
using NotificationService.Classes.Dto;

namespace NotificationService.Hubs.Implementation;

public class TestHub(
    ILogger<TestHub> logger
    ) : Hub<ITestHubReceiver>, ITestHub
{
    public async Task<TestEvent> SendTestEvent(TestEvent testEvent)
    {
        logger.LogTrace("SendTestEvent(testEvent={testEvent})", testEvent);

        await Clients.All.TestEventReceived(testEvent);
        return testEvent;
    }
}
