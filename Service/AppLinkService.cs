using Microsoft.Extensions.Options;
using NotificationService.Settings;

namespace NotificationService.Service;

public enum LinkMode
{
    Relative,
    Absolute
}

public class AppLinkService(ILogger<AppLinkService> logger, IOptions<FrontendSettings> frontendSettings)
{
    private string BaseUrl => frontendSettings.Value.BaseUrl.TrimEnd('/');

    public string BuildEventPageUrl(string eventId, LinkMode mode = LinkMode.Relative)
    {
        return $"{(mode == LinkMode.Absolute ? BaseUrl : "")}/events/{eventId}";
    }

    public string BuildEventsPageUrl(LinkMode mode = LinkMode.Relative)
    {
        return $"{(mode == LinkMode.Absolute ? BaseUrl : "")}/events";
    }

    public string BuildVolunteerDashboardPageUrl(string? eventId, LinkMode mode = LinkMode.Relative)
    {
        ArgumentNullException.ThrowIfNull(eventId);
        return $"{(mode == LinkMode.Absolute ? BaseUrl : "")}/events/{eventId}";
    }

    public string BuildVolunteerPlansPageUrl(string volunteerId, LinkMode mode = LinkMode.Relative)
    {
        return $"{(mode == LinkMode.Absolute ? BaseUrl : "")}/volunteers/{volunteerId}";
    }

    public string BuildTrustAlertPageUrl(LinkMode mode = LinkMode.Relative)
    {
        return $"{(mode == LinkMode.Absolute ? BaseUrl : "")}/trust";
    }

    public string BuildPlanAssignmentRequestsPageUrl(string? eventId, string? planId, LinkMode mode = LinkMode.Relative)
    {
        ArgumentNullException.ThrowIfNull(eventId);
        ArgumentNullException.ThrowIfNull(planId);
        return $"{(mode == LinkMode.Absolute ? BaseUrl : "")}/events/{eventId}/plans?planId={planId}&mode=assignments&status=REQUEST_FOR_ASSIGNMENT";
    }

    public string BuildPlanUnassignmentRequestsPageUrl(string? eventId, string? planId, LinkMode mode = LinkMode.Relative)
    {
        ArgumentNullException.ThrowIfNull(eventId);
        ArgumentNullException.ThrowIfNull(planId);
        return $"{(mode == LinkMode.Absolute ? BaseUrl : "")}/events/{eventId}/plans?planId={planId}&mode=assignments&status=AUCTION_REQUEST_FOR_UNASSIGN";
    }

    public string BuildPlanVolunteersPageUrl(string? eventId, string? planId, LinkMode mode = LinkMode.Relative)
    {
        ArgumentNullException.ThrowIfNull(eventId);
        ArgumentNullException.ThrowIfNull(planId);
        return $"{(mode == LinkMode.Absolute ? BaseUrl : "")}/events/{eventId}/plans?planId={planId}&mode=users";
    }
}
