using NotificationService.Classes;
using NotificationService.Service;
using NotificationService.ShiftserviceClient;
using ShiftControl.Events;

namespace NotificationService.NotificationProcessors.Volunteer;

public class UserUnlockNotificationProcessor(
    ILogger<UserUnlockNotificationProcessor> logger,
    ShiftserviceApiClientService clientService,
    AppLinkService appLinkService
) : INotificationProcessor<UserEvent>
{
    public async Task<PushNotification?> BuildPush(UserEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.PUSH,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_STATUS_CHANGED,
            RelatedVolunteerIds = {eventData.Volunteer.Id},
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        var date = DateTime.SpecifyKind(eventData.Timestamp?.DateTime ?? DateTime.UtcNow, DateTimeKind.Utc);

        return new PushNotification(
            recipients.Select(rec => rec.Volunteer.Id).ToList(),
            "Unlocked",
            $"Your account has been unlocked!\nYou are now able to sign up to shifts again.",
            date,
            appLinkService.BuildVolunteerDashboardPageUrl(eventData.ShiftPlanRefParts.ElementAtOrDefault(0)?.EventRefPart.Id.ToString())
            );
    }

    public async Task<EmailNotification?> BuildEmail(UserEvent eventData)
    {
        var recipients = await clientService.GetRecipientsForNotificationAsync(new()
        {
            NotificationChannel = RecipientsFilterDtoNotificationChannel.EMAIL,
            NotificationType = RecipientsFilterDtoNotificationType.VOLUNTEER_STATUS_CHANGED,
            RelatedVolunteerIds = {eventData.Volunteer.Id},
            ReceiverAccessLevel = RecipientsFilterDtoReceiverAccessLevel.VOLUNTEER
        });
        if (recipients.Count == 0) return null;

        return new EmailNotification(
            recipients.Select(rec => new EmailRecipientInfo(rec.Email, rec.Volunteer.FirstName, rec.Volunteer.LastName)).ToList(),
            "Unlocked",
            $"Your account has been unlocked!\nYou are now able to sign up to shifts again."
            );
    }
}
