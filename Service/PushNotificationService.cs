using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Classes;
using NotificationService.Classes.Dto;
using NotificationService.Database;
using NotificationService.Hubs;
using NotificationService.Hubs.Implementation;

namespace NotificationService.Service;

public class PushNotificationService(
    ILogger<PushNotificationService> logger,
    IHubContext<PushNotificationHub, IPushNotificationHubReceiver> notificationHub,
    NotificationServiceDbContext dbContext
    )
{
    public async Task PublishPushNotification(PushNotification notification)
    {
        logger.LogInformation("Sending push notification to users: {UserIds}",  notification.Recipients is null ? "everyone" : string.Join(", ", notification.Recipients));

        var notificationEvent = new PushNotificationDto(notification.Title, notification.Notification, notification.Time, notification.Url, notification.IsRead, notification.Id);

        notification = notification with { Recipients = ["28c02050-4f90-4f3a-b1df-3c7d27a166e8"] }; // TODO remove test code

        if (notification.Recipients is { } recipients)
        {
            /* persist only if known list of recipients */
            var individualNotifications = await SaveNotification(recipients, notification);
            foreach (var userNotification in individualNotifications)
            {
                var userId = userNotification.Recipients?.FirstOrDefault();
                if (userId is null) continue;

                var userNotificationEvent = new PushNotificationDto(userNotification.Title, userNotification.Notification, userNotification.Time, userNotification.Url, userNotification.IsRead, userNotification.Id);
                await notificationHub.Clients.User(userId).PushNotificationReceived(userNotificationEvent);
            }
        }
        else
        {
            await notificationHub.Clients.All.PushNotificationReceived(notificationEvent);
        }
    }

    private async Task<ICollection<PushNotification>> SaveNotification(ICollection<string> recipients, PushNotification notification)
    {
        ICollection<PushNotification> savedNotifications = new List<PushNotification>();

        foreach (var userId in recipients)
        {
            var entity = new Database.Model.PushNotificationEntity
            {
                RecipientId = userId,
                Title = notification.Title,
                Notification = notification.Notification,
                Time = notification.Time,
                Url = notification.Url,
                IsRead = notification.IsRead
            };

            await dbContext.PushNotifications.AddAsync(entity);
            savedNotifications.Add(notification with { Recipients = [userId], Id = entity.NotificationId });
        }

        await dbContext.SaveChangesAsync();

        /* truncate user notification count to 100 */
        foreach (var userId in recipients)
        {
            var userNotifications = dbContext.PushNotifications
                .Where(n => n.RecipientId == userId)
                .OrderByDescending(n => n.Time);

            var toRemove = await userNotifications
                .Skip(100)
                .ToListAsync();

            if (toRemove.Count != 0)
            {
                dbContext.PushNotifications.RemoveRange(toRemove);
            }
        }

        return savedNotifications;
    }

    public async Task<ICollection<PushNotification>> GetUserHistory(string userId)
    {
        var entities = dbContext.PushNotifications
            .Where(n => n.RecipientId == userId)
            .OrderByDescending(n => n.Time);

        return await entities
            .Select(n => new PushNotification(
                new [] {userId},
                n.Title,
                n.Notification,
                n.Time,
                n.Url,
                n.IsRead,
                n.NotificationId))
            .ToListAsync();
    }

    public async Task MarkAllAsRead(string userId)
    {
        var entities = dbContext.PushNotifications
            .Where(n => n.RecipientId == userId && !n.IsRead);

        await entities.ForEachAsync(n => n.IsRead = true);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> ClearHistory(string userId)
    {
        var entities = dbContext.PushNotifications
            .Where(n => n.RecipientId == userId);

        var count = await entities.CountAsync();

        dbContext.PushNotifications.RemoveRange(entities);
        await dbContext.SaveChangesAsync();

        return count;
    }

    public async Task ClearNotification(string userId, Guid notificationId)
    {
        var entity = await dbContext.PushNotifications
            .FirstOrDefaultAsync(n => n.RecipientId == userId && n.NotificationId == notificationId);

        if (entity != null)
        {
            dbContext.PushNotifications.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
