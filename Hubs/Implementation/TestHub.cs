using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Classes.Dto;

namespace NotificationService.Hubs.Implementation;

public class TestHub(
    ILogger<TestHub> logger
    ) : Hub<ITestHubReceiver>, ITestHub
{

    [Authorize]
    public async Task<TestEvent> SendTestEvent(TestEvent testEvent)
    {
        logger.LogTrace("SendTestEvent(testEvent={testEvent})", testEvent);

        var responseEvent = testEvent with { Message = testEvent.Message + " from " + Context.UserIdentifier };
        await Clients.All.TestEventReceived(responseEvent);
        return testEvent;
    }
}
