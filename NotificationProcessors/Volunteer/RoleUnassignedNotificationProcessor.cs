using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.Notifications;

public class RoleUnassignedNotificationProcessor(
    ILogger<RoleUnassignedNotificationProcessor> logger,
    ShiftserviceApiClientService clientService
) : INotificationProcessor<RoleVolunteerEvent>
{
    public Task<PushNotification?> BuildPush(RoleVolunteerEvent eventData)
    {
        return Task.FromResult<PushNotification?>(null);
    }

    public Task<EmailNotification?> BuildEmail(RoleVolunteerEvent eventData)
    {
        return Task.FromResult<EmailNotification?>(null);
    }
}
