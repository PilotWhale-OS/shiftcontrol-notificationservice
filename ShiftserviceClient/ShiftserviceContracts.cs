using System.Text.Json.Serialization;

namespace NotificationService.ShiftserviceClient;

public class AccountInfoDto
{
    public VolunteerDto Volunteer { get; init; } = null!;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Profile { get; init; }
    public UserType UserType { get; init; }
}

public class VolunteerDto
{
    public string Id { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

public class RecipientsDto
{
    public ICollection<AccountInfoDto> Recipients { get; init; } = [];
}

public class RecipientsFilterDto
{
    public RecipientsFilterDtoNotificationType NotificationType { get; init; }
    public RecipientsFilterDtoNotificationChannel NotificationChannel { get; init; }
    public RecipientsFilterDtoReceiverAccessLevel ReceiverAccessLevel { get; init; }
    public string? RelatedShiftPlanId { get; init; }
    public string? RelatedEventId { get; init; }
    public ICollection<string>? RelatedVolunteerIds { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserType
{
    ASSIGNED,
    ADMIN
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RecipientsFilterDtoNotificationChannel
{
    EMAIL,
    PUSH
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RecipientsFilterDtoReceiverAccessLevel
{
    PLANNER,
    VOLUNTEER,
    ADMIN
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RecipientsFilterDtoNotificationType
{
    VOLUNTEER_REQUEST_HANDLED,
    VOLUNTEER_TRADE_OR_AUCTION,
    VOLUNTEER_PLANS_CHANGED,
    VOLUNTEER_ROLES_CHANGED,
    VOLUNTEER_STATUS_CHANGED,
    VOLUNTEER_AUTO_ASSIGNED,
    VOLUNTEER_SHIFT_REMINDER,
    PLANNER_VOLUNTEER_REQUEST,
    PLANNER_VOLUNTEER_JOINED_PLAN,
    ADMIN_PLANNER_JOINED_PLAN,
    ADMIN_TRUST_ALERT_RECEIVED,
    ADMIN_REWARD_SYNC_USED
}
