using NotificationService.Service;

namespace NotificationService.Classes;

public static class NotificationProcessorsRegistration
{
    public static IServiceCollection AddNotificationProcessors(this IServiceCollection services, Func<NotificationProcessorsConfigBuilder, NotificationProcessorsConfig> configBuilderConfiguration)
    {
        services.AddScoped<EventProcessorService>();

        var builder = new NotificationProcessorsConfigBuilder();
        var config = configBuilderConfiguration(builder);

        // make config injectable
        services.AddSingleton(config);

        // make all processors injectable
        foreach (var processorType in config.GetAllNotificationProcessors())
        {
            services.AddTransient(processorType);
        }

        return services;
    }
}
