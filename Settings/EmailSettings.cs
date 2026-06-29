using MailKit.Security;

namespace NotificationService.Settings;

public class EmailSettings
{
    public required bool EnableSending { get; init; }
    public required string SmtpHost { get; init; }
    public required int SmtpPort { get; init; }
    public required string SmtpUsername { get; init; }
    public required string SmtpPassword { get; init; }
    public required string FromName { get; init; }
    public required string FromEmail { get; init; }
    public SecureSocketOptions SecureSocketOptions { get; init; } = SecureSocketOptions.StartTls;
}
