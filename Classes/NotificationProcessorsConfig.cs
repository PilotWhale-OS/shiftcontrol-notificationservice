using System.Text.RegularExpressions;

namespace NotificationService.Classes;


public class NotificationProcessorsConfigBuilder
{
    private readonly Dictionary<string, List<NotificationProcessorsConfig.EventProcessorMapping>> _eventNotificationProcessorsMap = new();

    public NotificationProcessorsConfigBuilder AddProcessor<TEvent, TProcessor>(string routingKey)
        where TEvent : class
        where TProcessor : class, INotificationProcessor<TEvent>
    {
        if (!_eventNotificationProcessorsMap.ContainsKey(routingKey))
        {
            _eventNotificationProcessorsMap[routingKey] = new List<NotificationProcessorsConfig.EventProcessorMapping>();
        }
        _eventNotificationProcessorsMap[routingKey].Add(new NotificationProcessorsConfig.EventProcessorMapping(routingKey, typeof(TEvent), typeof(TProcessor)));
        return this;
    }

    public NotificationProcessorsConfig Build()
    {
        return new NotificationProcessorsConfig(_eventNotificationProcessorsMap);
    }
}

public class NotificationProcessorsConfig(
    IReadOnlyDictionary<string, List<NotificationProcessorsConfig.EventProcessorMapping>> eventNotificationProcessorsMap
)
{
    public record struct EventProcessorMapping(string RoutingKeyPattern, Type EventType, Type ProcessorType);

    public ICollection<EventProcessorMapping>? GetEventProcessors(string routingKey)
    {
        foreach (var (pattern, processors) in eventNotificationProcessorsMap)
        {
            if (TopicMatch(pattern, routingKey))
            {
                return processors;
            }
        }

        return null;
    }

    public IReadOnlyList<Type> GetAllNotificationProcessors()
    {
        return eventNotificationProcessorsMap
            .Values
            .SelectMany(mappings => mappings)
            .Select(mapping => mapping.ProcessorType)
            .Distinct()
            .ToList();
    }


    private static bool TopicMatch(string pattern, string routingKey)
    {
        var regexPattern = "^" +
                           Regex.Escape(pattern)
                               .Replace(@"\*", "[^.]+")
                               .Replace(@"\#", "(.+)?")
                               .Replace(@"\.", @"\.") +
                           "$";

        return Regex.IsMatch(routingKey, regexPattern);
    }
}
