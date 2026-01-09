using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationService.Database.Model;
using NotificationService.Settings;

namespace NotificationService.Database;

public class NotificationServiceDbContext(IOptions<DbSettings> dbSettings) : DbContext
{
    public DbSet<PushNotificationEntity> PushNotifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(dbSettings.Value.ConnectionString);
    }
}
