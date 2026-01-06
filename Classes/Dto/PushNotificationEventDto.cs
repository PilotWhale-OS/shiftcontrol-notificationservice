using Tapper;

namespace NotificationService.Classes.Dto;

[TranspilationSource]
public record PushNotificationEventDto(string Title, string Notification, string? Url = null);
