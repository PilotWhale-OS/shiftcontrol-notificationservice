namespace NotificationService.Classes;

public record EmailNotification(ICollection<string> Recipients, string Subject, string Content);
