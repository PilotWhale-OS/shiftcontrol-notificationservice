using Tapper;

namespace NotificationService.Classes.Dto;

[TranspilationSource]
public record PushNotificationEventDto(string Title, string Notification, DateTime Time, string? Url = null);
