namespace NotificationService.Settings;

public class DbSettings
{
    public required string ConnectionString { get; init; }
    public required bool EnsureCreated { get; init; }
}
