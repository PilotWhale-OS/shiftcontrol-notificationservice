using Tapper;

namespace NotificationService.Classes.Dto;

[TranspilationSource]
public record PushNotificationDto(string Title, string Notification, DateTime Time, string? Url, bool IsRead, Guid? Id);
