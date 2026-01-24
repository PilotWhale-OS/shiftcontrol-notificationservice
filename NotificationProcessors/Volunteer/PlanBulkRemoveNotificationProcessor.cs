using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class PlanBulkRemoveNotificationProcessor(
    ILogger<PlanBulkRemoveNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<UserPlanBulkEvent>
{
    public Task<PushNotification?> BuildPush(UserPlanBulkEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(UserPlanBulkEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
