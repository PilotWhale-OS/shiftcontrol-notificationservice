using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using NotificationService.Settings;

namespace NotificationService.Service;

public class OAuthClientCredentialsTokenService(
    IHttpClientFactory httpClientFactory,
    IOptions<OAuthClientCredentialsSettings> settings,
    ILogger<OAuthClientCredentialsTokenService> logger)
{
    private static readonly TimeSpan RefreshSkew = TimeSpan.FromSeconds(30);
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private string? _cachedToken;
    private DateTimeOffset _refreshAfter = DateTimeOffset.MinValue;

    public async Task<string> ObtainToken(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        if (HasValidCachedToken(now))
        {
            return _cachedToken!;
        }

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            now = DateTimeOffset.UtcNow;
            if (HasValidCachedToken(now))
            {
                return _cachedToken!;
            }

            var tokenResponse = await RequestToken(cancellationToken);
            if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("OAuth2 token endpoint did not return an access token.");
            }

            var expiresIn = tokenResponse.ExpiresIn > 0 ? tokenResponse.ExpiresIn : 60;
            _cachedToken = tokenResponse.AccessToken;
            _refreshAfter = now.AddSeconds(expiresIn) - RefreshSkew;
            logger.LogInformation("Obtained OAuth2 client credentials token for notificationservice.");
            return _cachedToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private bool HasValidCachedToken(DateTimeOffset now)
    {
        return !string.IsNullOrWhiteSpace(_cachedToken) && now < _refreshAfter;
    }

    private async Task<TokenResponse> RequestToken(CancellationToken cancellationToken)
    {
        var options = settings.Value;
        if (string.IsNullOrWhiteSpace(options.TokenUrl)
            || string.IsNullOrWhiteSpace(options.ClientId)
            || string.IsNullOrWhiteSpace(options.ClientSecret))
        {
            throw new InvalidOperationException(
                "OAuth2 client credentials settings are incomplete. Configure OAuth2:Client or the legacy Keycloak section.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, options.TokenUrl)
        {
            Content = new FormUrlEncodedContent(BuildFormValues(options))
        };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{options.ClientId}:{options.ClientSecret}")));

        using var client = httpClientFactory.CreateClient();
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
        return payload ?? throw new InvalidOperationException("OAuth2 token endpoint returned an empty response.");
    }

    private static IEnumerable<KeyValuePair<string, string>> BuildFormValues(OAuthClientCredentialsSettings options)
    {
        yield return new KeyValuePair<string, string>("grant_type", "client_credentials");

        if (!string.IsNullOrWhiteSpace(options.Scope))
        {
            yield return new KeyValuePair<string, string>("scope", options.Scope.Trim());
        }
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; init; }
    }
}
