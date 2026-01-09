using Microsoft.Extensions.Options;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NotificationService.Settings;

namespace NotificationService.Service;

public class KeycloakService(IOptions<KeycloakSettings> settings)
{
    private string? _cachedToken = null;

    public async Task<string> ObtainToken()
    {
        if(_cachedToken is not null)
        {
            return _cachedToken;
        }

        var client = new KeycloakClient(settings.Value.BaseUrl);
        var token = await client.Auth.GetClientCredentialsTokenAsync(
            settings.Value.Realm,
            new KcClientCredentials
            {
                ClientId = settings.Value.ClientId,
                Secret = settings.Value.ClientSecret
            }
        );

        _cachedToken = token.Response.AccessToken;

        return _cachedToken;
    }
}
