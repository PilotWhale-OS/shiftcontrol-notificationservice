namespace NotificationService.Classes;

public record PushNotification(ICollection<string>? Recipients, string Title, string Notification, string? Url = null);
