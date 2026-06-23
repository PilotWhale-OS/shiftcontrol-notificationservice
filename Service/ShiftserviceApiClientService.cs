using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using NotificationService.Settings;
using NotificationService.ShiftserviceClient;

namespace NotificationService.Service;

public class ShiftserviceApiClientService(
    ILogger<ShiftserviceApiClientService> logger,
    HttpClient httpClient,
    IOptions<ShiftserviceSettings> apiOptions,
    OAuthClientCredentialsTokenService tokenService)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<ICollection<AccountInfoDto>> GetRecipientsForNotificationAsync(
        RecipientsFilterDto filterDto)
    {
        var response = await SendAsync<RecipientsDto>(HttpMethod.Get, "api/v1/me/recipients", filterDto);
        return response.Recipients;
    }

    public Task<AccountInfoDto> GetRecipientInfoAsync(string accountId)
    {
        return SendAsync<AccountInfoDto>(HttpMethod.Get, $"api/v1/me/recipients/{Uri.EscapeDataString(accountId)}");
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string relativeUrl, object? body = null)
    {
        EnsureBaseAddressConfigured();

        using var request = new HttpRequestMessage(method, relativeUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await tokenService.ObtainToken());

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            logger.LogError(
                "Shiftservice request to {Url} failed with status {StatusCode}: {Body}",
                request.RequestUri,
                (int)response.StatusCode,
                responseBody);
            response.EnsureSuccessStatusCode();
        }

        var payload = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return payload ?? throw new InvalidOperationException($"Shiftservice response for '{relativeUrl}' was empty.");
    }

    private void EnsureBaseAddressConfigured()
    {
        if (httpClient.BaseAddress is not null)
        {
            return;
        }

        var baseUrl = apiOptions.Value.BaseUrl;
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Shiftservice API base URL is not configured.");
        }

        httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
    }
}
