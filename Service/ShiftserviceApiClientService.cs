using Microsoft.Extensions.Options;
using NotificationService.Settings;

namespace NotificationService.Service;

public class ShiftserviceApiClientService(
    ILogger<ShiftserviceApiClientService> logger,
    HttpClient httpClient,
    IOptions<ShiftserviceSettings> apiOptions,
    KeycloakService keycloakService
    )
{
    public async Task<ShiftserviceClient.Client> GetClient()
    {
        var keycloakToken = await keycloakService.ObtainToken();

        // set auth header for all requests using this instance
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", keycloakToken);

        var baseUrl = apiOptions.Value.BaseUrl;
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("Typo API base URL is not configured.");
        }

        var apiClient = new ShiftserviceClient.Client(httpClient)
        {
            BaseUrl = baseUrl,
            ReadResponseAsString = false
        };
        return apiClient;
    }
}
