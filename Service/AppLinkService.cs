using Microsoft.Extensions.Options;
using NotificationService.Settings;

namespace NotificationService.Service;

public class AppLinkService(ILogger<AppLinkService> logger, IOptions<FrontendSettings> frontendSettings)
{
    private string BaseUrl => frontendSettings.Value.BaseUrl.TrimEnd('/');

    public string BuildEventPageUrl(string eventId)
    {
        return $"{BaseUrl}/events/{eventId}";
    }

    public string BuildEventsPageUrl()
    {
        return $"{BaseUrl}/events";
    }

    public string BuildVolunteerDashboardPageUrl(string? eventId)
    {
        ArgumentNullException.ThrowIfNull(eventId);
        return $"{BaseUrl}/events/{eventId}";
    }

    public string BuildVolunteerPlansPageUrl(string volunteerId)
    {
        return $"{BaseUrl}/volunteers/{volunteerId}";
    }

    public string BuildTrustAlertPageUrl()
    {
        return $"{BaseUrl}/trust";
    }

    public string BuildPlanAssignmentRequestsPageUrl(string? eventId, string? planId)
    {
        ArgumentNullException.ThrowIfNull(eventId);
        ArgumentNullException.ThrowIfNull(planId);
        return $"{BaseUrl}/events/{eventId}/plans/?planId={planId}&mode=assignments&status=REQUEST_FOR_ASSIGNMENT";
    }

    public string BuildPlanVolunteersPageUrl(string? eventId, string? planId)
    {
        ArgumentNullException.ThrowIfNull(eventId);
        ArgumentNullException.ThrowIfNull(planId);
        return $"{BaseUrl}/events/{eventId}/plans/?planId={planId}&mode=users";
    }
}
