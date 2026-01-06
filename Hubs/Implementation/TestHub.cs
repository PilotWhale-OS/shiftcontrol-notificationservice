using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Service;

namespace NotificationService.Hubs.Implementation;

public class TestHub(
    ILogger<TestHub> logger,
    TestService testService
    ) : Hub<ITestHubReceiver>, ITestHub
{

    [Authorize]
    public async Task<ActivityEvent> SendActivityCreatedEvent(ActivityEvent activityEvent)
    {
        logger.LogTrace("SendActivityCreatedEvent(activityEvent={activityEvent})", activityEvent);

        var responseEvent = activityEvent;// with
        // {
        //     Message = testEventDto.Message + " from " + Context.UserIdentifier + " ~ " + testService.GetMessage()
        // };

        await Clients.All.ActivityCreatedEventReceived(responseEvent);

        return activityEvent;
    }
}
