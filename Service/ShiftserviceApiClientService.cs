using Microsoft.Extensions.Options;
using NotificationService.Settings;
using NotificationService.ShiftserviceClient;

namespace NotificationService.Service;

public class ShiftserviceApiClientService(
    ILogger<ShiftserviceApiClientService> logger,
    HttpClient httpClient,
    IOptions<ShiftserviceSettings> apiOptions,
    KeycloakService keycloakService
    )
{
    private ShiftserviceClient.Client? _client = null;

    public async Task<ShiftserviceClient.Client> GetClient()
    {
        if (_client is not null)
        {
            return _client;
        }

        var keycloakToken = await keycloakService.ObtainToken();

        // set auth header for all requests using this instance
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", keycloakToken);

        var baseUrl = apiOptions.Value.BaseUrl;
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("Shiftservice API base URL is not configured.");
        }

        var apiClient = new ShiftserviceClient.Client(httpClient)
        {
            BaseUrl = baseUrl,
            ReadResponseAsString = false
        };

        _client = apiClient;
        return apiClient;
    }

    public async Task<ICollection<AccountInfoDto>> GetRecipientsForNotificationAsync(
        RecipientsFilterDto filterDto)
    {
        var client = await GetClient();
        var response = await client.GetRecipientsForNotificationAsync(filterDto);
        return response.Recipients;
    }

    public async Task<AccountInfoDto> GetRecipientInfoAsync(string accountId)
    {
        var client = await GetClient();
        var response = await client.GetRecipientInformationAsync(accountId);
        return response;
    }
}
