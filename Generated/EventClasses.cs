namespace NotificationService.Generated
{

    public partial class ActivityEvent
    {
        public string ActingUserId { get; set; }
        public Activity Activity { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class Activity
    {
        public string Description { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string Id { get; set; }
        public ActivityLocation Location { get; set; }
        public string Name { get; set; }
        public bool? ReadOnly { get; set; }
        public DateTimeOffset? StartTime { get; set; }
    }

    public partial class ActivityLocation
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? ReadOnly { get; set; }
        public string Url { get; set; }
    }

    public partial class ActivityPart
    {
        public string Description { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string Id { get; set; }
        public ActivityPartLocation Location { get; set; }
        public string Name { get; set; }
        public bool? ReadOnly { get; set; }
        public DateTimeOffset? StartTime { get; set; }
    }

    public partial class ActivityPartLocation
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? ReadOnly { get; set; }
        public string Url { get; set; }
    }

    public partial class AssignmentEvent
    {
        public string ActingUserId { get; set; }
        public Assignmentent Assignment { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class Assignmentent
    {
        public AssignmentPositionSlot PositionSlot { get; set; }
        public AssignmentStatus? Status { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class AssignmentPositionSlot
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class AssignmentPart
    {
        public AssignmentPartPositionSlot PositionSlot { get; set; }
        public AssignmentStatus? Status { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class AssignmentPartPositionSlot
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class AssignmentPartBuilder
    {
    }

    public partial class AssignmentSwitchEvent
    {
        public string ActingUserId { get; set; }
        public AssignmentSwitchEventOfferingAssignment OfferingAssignment { get; set; }
        public AssignmentSwitchEventOfferingAssignment RequestedAssignment { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class AssignmentSwitchEventOfferingAssignment
    {
        public PurplePositionSlot PositionSlot { get; set; }
        public AssignmentStatus? Status { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class PurplePositionSlot
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class EventEvent
    {
        public string ActingUserId { get; set; }
        public Eventevent Event { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class Eventevent
    {
        public DateTimeOffset? EndTime { get; set; }
        public string Id { get; set; }
        public string LongDescription { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public DateTimeOffset? StartTime { get; set; }
    }

    public partial class EventPart
    {
        public DateTimeOffset? EndTime { get; set; }
        public string Id { get; set; }
        public string LongDescription { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public DateTimeOffset? StartTime { get; set; }
    }

    public partial class InvitePart
    {
        public bool? Active { get; set; }
        public AutoAssignedRoleElement[] AutoAssignedRoles { get; set; }
        public string Code { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string Id { get; set; }
        public long? MaxUses { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public InvitePartType? Type { get; set; }
        public long? UsedCount { get; set; }
    }

    public partial class AutoAssignedRoleElement
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? SelfAssignable { get; set; }
        public string ShiftPlanId { get; set; }
    }

    public partial class LocationEvent
    {
        public string ActingUserId { get; set; }
        public LocationEventLocation Location { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class LocationEventLocation
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? ReadOnly { get; set; }
        public string Url { get; set; }
    }

    public partial class LocationPart
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? ReadOnly { get; set; }
        public string Url { get; set; }
    }

    public partial class NotificationSettingsEvent
    {
        public string ActingUserId { get; set; }
        public NotificationSettings NotificationSettings { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class NotificationSettings
    {
        public ChannelElement[] Channels { get; set; }
        public NotificationSettingsType? Type { get; set; }
    }

    public partial class NotificationSettingsPart
    {
        public ChannelElement[] Channels { get; set; }
        public NotificationSettingsType? Type { get; set; }
    }

    public partial class PositionSlotEvent
    {
        public string ActingUserId { get; set; }
        public PositionSlotEventPositionSlot PositionSlot { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class PositionSlotEventPositionSlot
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class PositionSlotPart
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class PositionSlotPartBuilder
    {
    }

    public partial class PositionSlotVolunteerEvent
    {
        public string ActingUserId { get; set; }
        public PositionSlotVolunteerEventPositionSlot PositionSlot { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class PositionSlotVolunteerEventPositionSlot
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class PreferenceEvent
    {
        public string ActingUserId { get; set; }
        public PreferenceEventPositionSlot PositionSlot { get; set; }
        public long? PreferenceLevel { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class PreferenceEventPositionSlot
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class RoleEvent
    {
        public string ActingUserId { get; set; }
        public RoleEventRole Role { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class RoleEventRole
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? SelfAssignable { get; set; }
        public string ShiftPlanId { get; set; }
    }

    public partial class RolePart
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? SelfAssignable { get; set; }
        public string ShiftPlanId { get; set; }
    }

    public partial class RoleVolunteerEvent
    {
        public string ActingUserId { get; set; }
        public RoleVolunteerEventRole Role { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class RoleVolunteerEventRole
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? SelfAssignable { get; set; }
        public string ShiftPlanId { get; set; }
    }

    public partial class ShiftEvent
    {
        public string ActingUserId { get; set; }
        public Shift Shift { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class Shift
    {
        public DateTimeOffset? EndTime { get; set; }
        public string Id { get; set; }
        public long? LocationId { get; set; }
        public string LongDescription { get; set; }
        public string Name { get; set; }
        public PositionSlotElement[] PositionSlots { get; set; }
        public long? RelatedActivityId { get; set; }
        public string ShortDescription { get; set; }
        public DateTimeOffset? StartTime { get; set; }
    }

    public partial class PositionSlotElement
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class ShiftPart
    {
        public DateTimeOffset? EndTime { get; set; }
        public string Id { get; set; }
        public long? LocationId { get; set; }
        public string LongDescription { get; set; }
        public string Name { get; set; }
        public PositionSlotClass[] PositionSlots { get; set; }
        public long? RelatedActivityId { get; set; }
        public string ShortDescription { get; set; }
        public DateTimeOffset? StartTime { get; set; }
    }

    public partial class PositionSlotClass
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class ShiftPlanEvent
    {
        public string ActingUserId { get; set; }
        public ShiftPlanEventShiftPlan ShiftPlan { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class ShiftPlanEventShiftPlan
    {
        public string Id { get; set; }
        public LockStatus? LockStatus { get; set; }
        public string LongDescription { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
    }

    public partial class ShiftPlanInviteEvent
    {
        public string ActingUserId { get; set; }
        public Invite Invite { get; set; }
        public ShiftPlanInviteEventShiftPlan ShiftPlan { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class Invite
    {
        public bool? Active { get; set; }
        public AutoAssignedRoleClass[] AutoAssignedRoles { get; set; }
        public string Code { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string Id { get; set; }
        public long? MaxUses { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public InvitePartType? Type { get; set; }
        public long? UsedCount { get; set; }
    }

    public partial class AutoAssignedRoleClass
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool? SelfAssignable { get; set; }
        public string ShiftPlanId { get; set; }
    }

    public partial class ShiftPlanInviteEventShiftPlan
    {
        public string Id { get; set; }
        public LockStatus? LockStatus { get; set; }
        public string LongDescription { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
    }

    public partial class ShiftPlanPart
    {
        public string Id { get; set; }
        public LockStatus? LockStatus { get; set; }
        public string LongDescription { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
    }

    public partial class ShiftPlanVolunteerEvent
    {
        public string ActingUserId { get; set; }
        public ShiftPlanVolunteerEventShiftPlan ShiftPlan { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class ShiftPlanVolunteerEventShiftPlan
    {
        public string Id { get; set; }
        public LockStatus? LockStatus { get; set; }
        public string LongDescription { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
    }

    public partial class TimeConstraintEvent
    {
        public string ActingUserId { get; set; }
        public TimeConstraint TimeConstraint { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
    }

    public partial class TimeConstraint
    {
        public DateTimeOffset? From { get; set; }
        public string Id { get; set; }
        public DateTimeOffset? To { get; set; }
        public TimeConstraintType? Type { get; set; }
    }

    public partial class TimeConstraintPart
    {
        public DateTimeOffset? From { get; set; }
        public string Id { get; set; }
        public DateTimeOffset? To { get; set; }
        public TimeConstraintType? Type { get; set; }
    }

    public partial class TradeEvent
    {
        public string ActingUserId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string TraceId { get; set; }
        public Trade Trade { get; set; }
    }

    public partial class Trade
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public TradeOfferingAssignment OfferingAssignment { get; set; }
        public TradeOfferingAssignment RequestedAssignment { get; set; }
        public TradeStatus? Status { get; set; }
    }

    public partial class TradeOfferingAssignment
    {
        public FluffyPositionSlot PositionSlot { get; set; }
        public AssignmentStatus? Status { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class FluffyPositionSlot
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public partial class TradePart
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public OfferingAssignment OfferingAssignment { get; set; }
        public OfferingAssignment RequestedAssignment { get; set; }
        public TradeStatus? Status { get; set; }
    }

    public partial class OfferingAssignment
    {
        public TentacledPositionSlot PositionSlot { get; set; }
        public AssignmentStatus? Status { get; set; }
        public string VolunteerId { get; set; }
    }

    public partial class TentacledPositionSlot
    {
        public string PositionSlotDescription { get; set; }
        public long? PositionSlotId { get; set; }
        public string PositionSlotName { get; set; }
    }

    public enum AssignmentStatus { Accepted, Auction, AuctionRequestForUnassign, RequestForAssignment };

    public enum InvitePartType { PlannerJoin, VolunteerJoin };

    public enum ChannelElement { Email, Push };

    public enum NotificationSettingsType { AutoAssigned, ShiftReminder, TradeAcceptedDeclined, TradeRequested };

    public enum LockStatus { Locked, SelfSignup, Supervised };

    public enum TimeConstraintType { Emergency, Unavailable };

    public enum TradeStatus { Accepted, Canceled, Open, Rejected };
}
