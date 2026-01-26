namespace NotificationService.Classes;

public record PushNotification(ICollection<string>? Recipients, string Title, string Notification, DateTime Time, string? Url, bool IsRead = false, Guid? Id = null);
