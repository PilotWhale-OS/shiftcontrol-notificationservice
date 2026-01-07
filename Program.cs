﻿using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NotificationService.Classes;
using NotificationService.Generated;
using NotificationService.Hubs.Implementation;
using NotificationService.Notifications;
using NotificationService.Service;
using NotificationService.Settings;

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
            .AddSignalR().Services
            .Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"))
            .AddNotificationProcessors(pb => pb
                .AddProcessor<ActivityEvent, ActivityCreatedNotificationProcessor>("shiftcontrol.activity.created")
                .AddProcessor<ActivityEvent, ActivityUpdatedNotificationProcessor>("shiftcontrol.activity.updated.*")
                .Build()
            )
            .AddScoped<PushNotificationService>()
            .AddScoped<EventProcessorService>()
            .AddHostedService<RabbitMqService>()
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

        return builder.Build();
    }

    private static void SetupRoutes(WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
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
