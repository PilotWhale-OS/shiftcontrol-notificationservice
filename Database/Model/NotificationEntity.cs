using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Database.Model;

[Index(nameof(RecipientId), nameof(Time))]
public class PushNotificationEntity
{
    [Key]
    public Guid NotificationId { get; set; }

    [Required]
    public bool IsRead { get; set; }

    [Required]
    public string RecipientId { get; set; }

    [Required]
    public string Notification { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public DateTime Time { get; set; }

    public string? Url { get; set; }
}
