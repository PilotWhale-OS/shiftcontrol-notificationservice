using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Classes.Dto;
using NotificationService.Service;

namespace NotificationService.Hubs.Implementation;

public class TestHub(
    ILogger<TestHub> logger,
    TestService testService
    ) : Hub<ITestHubReceiver>, ITestHub
{

    [Authorize]
    public async Task<TestEventDto> SendTestEvent(TestEventDto testEventDto)
    {
        logger.LogTrace("SendTestEvent(testEventDto={testEventDto})", testEventDto);

        var responseEvent = testEventDto with
        {
            Message = testEventDto.Message + " from " + Context.UserIdentifier + " ~ " + testService.GetMessage()
        };
        await Clients.All.TestEventReceived(responseEvent);
        return testEventDto;
    }
}
