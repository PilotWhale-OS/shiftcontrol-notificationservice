using NotificationService.Classes;
using NotificationService.Generated;
using NotificationService.Service;

namespace NotificationService.Notifications;

public class ActivityCreatedNotificationProcessor(
    ILogger<ActivityCreatedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<ActivityEvent>
{
    public async Task<PushNotification?> BuildPush(ActivityEvent activityEvent)
    {
        var client = await clientService.GetClient();

        return new PushNotification(
            null,
            "New Activity Created",
            $"Activity '{activityEvent.Activity.Name}' has been created.",
            DateTime.UtcNow,
            null,
            false,
            null
            );
    }

    public Task<EmailNotification?> BuildEmail(ActivityEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
