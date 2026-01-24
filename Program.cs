﻿using System.Globalization;
 using System.Threading.Channels;
 using Microsoft.AspNetCore.Authentication.JwtBearer;
using NotificationService.Classes;
using NotificationService.Database;
using NotificationService.Hubs.Implementation;
using NotificationService.Notifications;
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

        builder.Services
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConfiguration(builder.Configuration.GetSection("Logging"))
                .AddConsole())
            .AddCors()
            .AddHttpClient()
            .AddHttpClient<ShiftserviceApiClientService>().Services
            .AddSignalR().Services
            .Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"))
            .Configure<DbSettings>(builder.Configuration.GetSection("Db"))
            .Configure<KeycloakSettings>(builder.Configuration.GetSection("Keycloak"))
            .Configure<ShiftserviceSettings>(builder.Configuration.GetSection("Shiftservice"))
            .Configure<EmailSettings>(builder.Configuration.GetSection("Email"))
            .AddDbContext<NotificationServiceDbContext>()
            .AddNotificationProcessors(pb => pb
                // ADMIN
                .AddProcessor<ShiftPlanVolunteerEvent, PlannerJoinedPlanNotificationProcessor>("shiftcontrol.shiftplan.joined.planner.#")
                .AddProcessor<TrustAlertEvent, TrustAlertNotificationProcessor>("shiftcontrol.trustalert.received.#")
                // PLANNER
                .AddProcessor<PositionSlotVolunteerEvent, RequestJoinNotificationProcessor>("shiftcontrol.positionslot.request.join.created.#")
                .AddProcessor<PositionSlotVolunteerEvent, RequestLeaveNotificationProcessor>("shiftcontrol.positionslot.request.leave.created.#")
                .AddProcessor<ShiftPlanVolunteerEvent, VolunteerJoinedPlanNotificationProcessor>("shiftcontrol.shiftplan.joined.volunteer.#")
                // VOLUNTEER
                .AddProcessor<AssignmentEvent, AuctionClaimedNotificationProcessor>("shiftcontrol.auction.claimed.#")
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
                .Build()
            )
            .AddSingleton<KeycloakService>()
            .AddScoped<PushNotificationService>()
            .AddScoped<EventProcessorService>()
            .AddScoped<ShiftserviceApiClientService>()
            .AddScoped<MailService>()
            .AddSingleton(Channel.CreateUnbounded<EmailNotification>())
            .AddHostedService<RabbitMqService>()
            .AddHostedService<MailClient>()
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

        var kc = app.Services
            .GetRequiredService<KeycloakService>();
        var token = kc.ObtainToken().Result;
        logger.LogInformation("Obtained Keycloak token: {token}", token);


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
}
