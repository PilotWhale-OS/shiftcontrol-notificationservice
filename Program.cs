using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NotificationService.Hubs.Implementation;

namespace NotificationService;

class Program
{
    private const string HubPrefix = "/hubs";
    static async Task Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        var host = CreateHost(args);
        SetupRoutes(host);
        await host.RunAsync();
    }

    private static WebApplication CreateHost(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddCors()
            .AddSignalR().Services
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConfiguration(builder.Configuration.GetSection("Logging"))
                .AddConsole())
            .BuildServiceProvider();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(builder.Configuration.GetRequiredSection("SignalR").GetValue<int>("HostPort"));
        });

        // see https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-10.0#built-in-jwt-authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = "http://keycloak:8080/realms/dev";
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new()
            {
                ValidateAudience = false,
                ValidIssuers = ["http://keycloak:8080/realms/dev", "http://keycloak.127.0.0.1.nip.io/realms/dev"]
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(HubPrefix))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        return builder.Build();
    }

    private static void SetupRoutes(WebApplication app)
    {
        app.MapHub<TestHub>(HubPrefix + "/testhub");

        app.UseCors(options =>
        {
            options.WithOrigins("http://localhost:4200").AllowCredentials().WithHeaders("*").WithMethods("*");
        });

        app.UseAuthentication();
        app.UseAuthorization();
    }
}
