using System.Globalization;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using NotificationService.Classes;
using NotificationService.Database;
using NotificationService.Hubs.Implementation;
using NotificationService.NotificationProcessors.Admin;
using NotificationService.NotificationProcessors.Planner;
using NotificationService.NotificationProcessors.Volunteer;
using NotificationService.Service;
using NotificationService.Settings;
using ShiftControl.Events;

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

        builder.Services.AddLogging(loggingBuilder => loggingBuilder
            .AddConfiguration(builder.Configuration.GetSection("Logging"))
            .AddConsole());
        builder.Services.AddCors();
        builder.Services.AddHttpClient();
        builder.Services.AddHttpClient<ShiftserviceApiClientService>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<ShiftserviceSettings>>().Value;
            if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                client.BaseAddress = new Uri(settings.BaseUrl, UriKind.Absolute);
            }
        });
        builder.Services.AddSignalR();
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Db"));
        builder.Services.Configure<ShiftserviceSettings>(builder.Configuration.GetSection("Shiftservice"));
        builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("Frontend"));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
        builder.Services.AddOptions<OAuthClientCredentialsSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("OAuth2:Client").Bind(settings);

                if (string.IsNullOrWhiteSpace(settings.TokenUrl))
                {
                    var legacyBaseUrl = configuration["Keycloak:BaseUrl"];
                    var legacyRealm = configuration["Keycloak:Realm"];
                    if (!string.IsNullOrWhiteSpace(legacyBaseUrl) && !string.IsNullOrWhiteSpace(legacyRealm))
                    {
                        settings.TokenUrl = BuildLegacyTokenUrl(legacyBaseUrl, legacyRealm);
                    }
                }

                settings.ClientId ??= configuration["Keycloak:ClientId"];
                settings.ClientSecret ??= configuration["Keycloak:ClientSecret"];
                settings.Scope ??= configuration["Keycloak:Scope"];
            });
        builder.Services.AddDbContext<NotificationServiceDbContext>();
        builder.Services.AddNotificationProcessors(pb => pb
            // ADMIN
            .AddProcessor<ShiftPlanVolunteerEvent, PlannerJoinedPlanNotificationProcessor>("shiftcontrol.shiftplan.joined.planner.#")
            .AddProcessor<TrustAlertEvent, TrustAlertNotificationProcessor>("shiftcontrol.trustalert.received.#")
            // PLANNER
            .AddProcessor<PositionSlotVolunteerEvent, RequestJoinNotificationProcessor>("shiftcontrol.positionslot.request.join.created.#")
            .AddProcessor<PositionSlotVolunteerEvent, RequestLeaveNotificationProcessor>("shiftcontrol.positionslot.request.leave.created.#")
            .AddProcessor<ShiftPlanVolunteerEvent, VolunteerJoinedPlanNotificationProcessor>("shiftcontrol.shiftplan.joined.volunteer.#")
            // VOLUNTEER
            .AddProcessor<ClaimedAuctionEvent, AuctionClaimedNotificationProcessor>("shiftcontrol.auction.claimed.#")
            .AddProcessor<UserEventBulkEvent, EventBulkAddNotificationProcessor>("shiftcontrol.users.bulk.add")
            .AddProcessor<UserEventBulkEvent, EventBulkRemoveNotificationProcessor>("shiftcontrol.users.bulk.remove")
            .AddProcessor<UserEvent, EventBulkUpdateNotificationProcessor>("shiftcontrol.users.#.update")
            .AddProcessor<UserPlanBulkEvent, PlanBulkAddNotificationProcessor>("shiftcontrol.shift-plans.#.users.bulk.add")
            .AddProcessor<UserPlanBulkEvent, PlanBulkRemoveNotificationProcessor>("shiftcontrol.shift-plans.#.users.bulk.remove")
            .AddProcessor<UserEvent, PlanBulkUpdateNotificationProcessor>("shiftcontrol.shift-plans.#.users.#")
            .AddProcessor<PositionSlotVolunteerEvent, RequestJoinAcceptedNotificationProcessor>("shiftcontrol.positionslot.request.join.accepted.#")
            .AddProcessor<PositionSlotVolunteerEvent, RequestJoinDeclinedNotificationProcessor>("shiftcontrol.positionslot.request.join.declined.#")
            .AddProcessor<PositionSlotVolunteerEvent, RequestLeaveAcceptedNotificationProcessor>("shiftcontrol.positionslot.request.leave.accepted.#")
            .AddProcessor<PositionSlotVolunteerEvent, RequestLeaveDeclinedNotificationProcessor>("shiftcontrol.positionslot.request.leave.declined.#")
            .AddProcessor<RoleVolunteerEvent, RoleAssignedNotificationProcessor>("shiftcontrol.role.assigned.#")
            .AddProcessor<RoleVolunteerEvent, RoleUnassignedNotificationProcessor>("shiftcontrol.role.unassigned.#")
            .AddProcessor<AssignmentSwitchEvent, TradeCompletedNotificationProcessor>("shiftcontrol.trade.request.completed.#")
            .AddProcessor<TradeEvent, TradeCreatedNotificationProcessor>("shiftcontrol.trade.request.created.#")
            .AddProcessor<TradeEvent, TradeDeclinedNotificationProcessor>("shiftcontrol.trade.request.declined.#")
            .AddProcessor<UserEvent, UserLockNotificationProcessor>("shiftcontrol.users.#.lock")
            .AddProcessor<UserEvent, UserResetNotificationProcessor>("shiftcontrol.users.#.unlock")
            .AddProcessor<UserEvent, UserUnlockNotificationProcessor>("shiftcontrol.users.#.reset")
            .Build());
        builder.Services.AddSingleton<OAuthClientCredentialsTokenService>();
        builder.Services.AddSingleton<AppLinkService>();
        builder.Services.AddScoped<PushNotificationService>();
        builder.Services.AddScoped<EventProcessorService>();
        builder.Services.AddScoped<MailService>();
        builder.Services.AddSingleton(Channel.CreateUnbounded<EmailNotification>());
        builder.Services.AddHostedService<RabbitMqService>();
        builder.Services.AddHostedService<MailClient>();

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
            options.Authority = builder.Configuration.GetRequiredSection("Jwt").GetValue<string>("Authority");
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new()
            {
                ValidateAudience = false,
                ValidIssuers = builder.Configuration.GetRequiredSection("Jwt:Issuers").Get<string[]>(),
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

        var app = builder.Build();

        /* ensure db created */
        var dbSettings = builder.Configuration.GetSection("Db").Get<DbSettings>();
        if (dbSettings?.EnsureCreated == true)
        {
            var db = app.Services
                .GetRequiredService<NotificationServiceDbContext>();
            db.Database.EnsureCreated();
        }

        return app;
    }

    private static void SetupRoutes(WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Notificationservice routes configured.");

        app.MapHub<PushNotificationHub>(HubPrefix + "/push");

        app.UseCors(options =>
        {
            var origins = app.Configuration
                .GetRequiredSection("SignalR:AllowedOrigins")
                .Get<string[]>() ?? [];
            logger.LogDebug("Configuring CORS for origins: {origins}", string.Join(",", origins));
            options.WithOrigins(origins).AllowCredentials().WithHeaders("*").WithMethods("*");
        });

        app.UseAuthentication();
        app.UseAuthorization();
    }

    private static string BuildLegacyTokenUrl(string baseUrl, string realm)
    {
        var normalizedBaseUrl = baseUrl.Trim().TrimEnd('/');
        return $"{normalizedBaseUrl}/realms/{realm.Trim()}/protocol/openid-connect/token";
    }
}
