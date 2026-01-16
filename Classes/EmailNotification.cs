namespace NotificationService.Classes;

public record EmailRecipientInfo(string Email, string FirstName, string LastName);
public record EmailNotification(ICollection<EmailRecipientInfo> Recipients, string Subject, string Content);
