using NotificationService.Classes;
using NotificationService.Generated;

namespace NotificationService.Notifications;

public class ActivityCreatedNotificationProcessor(
    ILogger<ActivityCreatedNotificationProcessor> logger
) : INotificationProcessor<ActivityEvent>
{
    public Task<PushNotification?> BuildPush(ActivityEvent eventData)
    {
        logger.LogInformation("Building push notification");
        return Task.FromResult<PushNotification?>(
            new PushNotification(null, "New Activity Created", $"Activity '{eventData.Activity.Name}' has been created.")
        );
    }

    public Task<EmailNotification?> BuildEmail(ActivityEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
