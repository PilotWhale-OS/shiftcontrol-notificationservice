using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NotificationService.Classes;
using NotificationService.Converters;
using RabbitMQ.Client.Events;
using ShiftControl.Events;

namespace NotificationService.Service;

public class EventProcessorService(
    ILogger<EventProcessorService> logger,
    NotificationProcessorsConfig notificationProcessorsConfig,
    IServiceProvider provider)
{
    /// <summary>
    /// Deserialize an event by routing key and registered event types
    /// Selects fitting processors for the event type and invokes them to build messages and get recipients
    /// </summary>
    /// <param name="model"></param>
    /// <param name="eventArgs"></param>
    public async Task HandleEventAsync(object model, BasicDeliverEventArgs eventArgs)
    {
        logger.LogDebug("Received event with routing key {routingKey}", eventArgs.RoutingKey);

        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        var eventType = notificationProcessorsConfig.GetEventType(eventArgs.RoutingKey);
        if (eventType is null)
        {
            logger.LogInformation("Ignoring event with unknown routing key {routingKey}", eventArgs.RoutingKey);
            return;
        }

        object? eventData = null;
        try
        {
            /* should have a static FromJson method if quicktype class -> get using reflection */
            var fromJsonMethod = eventType.GetMethod("FromJson", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (fromJsonMethod is not null)
            {
                Converter.Settings.Converters = new List<JsonConverter>(new[] { new UnixTimestampConverter() }
                    .Concat(
                        Converter.Settings.Converters
                            .Where(c => c.GetType() != typeof(IsoDateTimeConverter))
                    ));
                eventData = fromJsonMethod.Invoke(null, [message]);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error deserializing event");
        }

        if (eventData is null)
        {
            logger.LogWarning("Failed to deserialize event data for routing key {routingKey}", eventArgs.RoutingKey);
            return;
        }

        logger.LogInformation("Deserialized event {routingKey}:{eventType}: {eventData}", eventArgs.RoutingKey, eventType, eventData);

        var processors = notificationProcessorsConfig.GetEventNotificationProcessors(eventType);


        // i f'd up with the generics here so using reflection to call the methods
        logger.LogDebug("Processing event {eventType} using {processorCount} processors", eventType, processors.Count);
        var tasks = processors.Select(processorType => Task.Run(async () =>
        {
            logger.LogInformation("Processing message using processor {processorType}", processorType);

            try
            {
                var scope = provider.CreateScope();
                var pushNotificationService = scope.ServiceProvider.GetRequiredService<PushNotificationService>(); /*  avoid db concurrency on same instance*/
                var processor = scope.ServiceProvider.GetRequiredService(processorType);

                var buildPushMethod = processorType.GetMethod("BuildPush");
                if (buildPushMethod is null)
                {
                    logger.LogError("Processor {processorType} does not have BuildPush method", processorType);
                    return;
                }

                var pushMessageTask = (Task<PushNotification?>)buildPushMethod.Invoke(processor, [eventData])!;
                var pushMessage = await pushMessageTask;

                var buildEmailMethod = processorType.GetMethod("BuildEmail");
                if (buildEmailMethod is null)
                {
                    logger.LogError("Processor {processorType} does not have BuildEmail method", processorType);
                    return;
                }

                var emailMessageTask = (Task<EmailNotification?>)buildEmailMethod.Invoke(processor, [eventData])!;
                var emailMessage = await emailMessageTask;

                logger.LogInformation("Push Message: {notificationMessage}", pushMessage);
                logger.LogInformation("Email Message: {notificationMessage}", emailMessage);

                if(pushMessage is not null) await pushNotificationService.PublishPushNotification(pushMessage);
                scope.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing notification with processor {processorType}", processorType);
            }
        }));

        await Task.WhenAll(tasks);
        logger.LogDebug("Completed processing event with routing key {routingKey}", eventArgs.RoutingKey);
    }
}
