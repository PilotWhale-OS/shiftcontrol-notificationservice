using System.Text;
using Newtonsoft.Json;
using NotificationService.Classes;
using NotificationService.Converters;
using RabbitMQ.Client.Events;

namespace NotificationService.Service;

public class EventProcessorService(
    ILogger<EventProcessorService> logger,
    NotificationProcessorsConfig notificationProcessorsConfig,
    PushNotificationService pushNotificationService,
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

        var eventData = JsonConvert.DeserializeObject(message, eventType, new JsonSerializerSettings
        {
            Converters = { new UnixTimestampConverter() }
        });

        if (eventData is null)
        {
            logger.LogWarning("Failed to deserialize event data for routing key {routingKey}", eventArgs.RoutingKey);
            return;
        }

        logger.LogDebug("Deserialized event {routingKey}:{eventType}: {eventData}", eventArgs.RoutingKey, eventType, eventData);

        var processors = notificationProcessorsConfig.GetEventNotificationProcessors(eventType);

        var scope = provider.CreateScope();

        // i f'd up with the generics here so using reflection to call the methods
        logger.LogInformation("Processing event {eventType} using {processorCount} processors", eventType, processors.Count);
        var tasks = processors.Select(processorType => Task.Run(async () =>
        {
            logger.LogDebug("Processing message for processor {processorType}", processorType);

            try
            {
                var processor = scope.ServiceProvider.GetRequiredService(processorType);

                var buildPushMethod = processorType.GetMethod("BuildPush");
                if (buildPushMethod is null)
                {
                    logger.LogError("Processor {processorType} does not have BuildPush method", processorType);
                    return;
                }

                var pushMessageTask = (Task<PushNotification?>)buildPushMethod.Invoke(processor, new[] { eventData })!;
                var pushMessage = await pushMessageTask;

                var buildEmailMethod = processorType.GetMethod("BuildEmail");
                if (buildEmailMethod is null)
                {
                    logger.LogError("Processor {processorType} does not have BuildEmail method", processorType);
                    return;
                }

                var emailMessageTask = (Task<EmailNotification?>)buildEmailMethod.Invoke(processor, new[] { eventData })!;
                var emailMessage = await emailMessageTask;

                logger.LogDebug("Push Message: {notificationMessage}", pushMessage);
                logger.LogDebug("Email Message: {notificationMessage}", emailMessage);

                if(pushMessage is not null) await pushNotificationService.SendPushNotification(pushMessage);
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
