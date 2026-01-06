using NotificationService.Classes;
using NotificationService.Generated;

namespace NotificationService.Notifications;

public class ActivityUpdatedNotificationProcessor(
    ILogger<ActivityCreatedNotificationProcessor> logger
) : INotificationProcessor<ActivityEvent>
{
    public Task<PushNotification?> BuildPush(ActivityEvent eventData)
    {
        return Task.FromResult<PushNotification?>(
            new PushNotification(null, "Activity Updated", $"Activity '{eventData.Activity.Name}' has been updated.")
        );
    }

    public Task<EmailNotification?> BuildEmail(ActivityEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
