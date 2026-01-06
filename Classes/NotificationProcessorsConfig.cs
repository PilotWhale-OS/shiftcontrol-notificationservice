namespace NotificationService.Classes;


public class NotificationProcessorsConfigBuilder
{
    private readonly Dictionary<string, Type> _eventTypeMap = new();
    private readonly Dictionary<Type, List<Type>> _eventNotificationProcessorsMap = new();

    public NotificationProcessorsConfigBuilder AddProcessor<TEvent, TProcessor>(string routingKey)
        where TEvent : class
        where TProcessor : class, INotificationProcessor<TEvent>
    {
        _eventTypeMap[routingKey] = typeof(TEvent);

        if (!_eventNotificationProcessorsMap.TryGetValue(typeof(TEvent), out var processors))
        {
            processors = new List<Type>();
            _eventNotificationProcessorsMap[typeof(TEvent)] = processors;
        }

        processors.Add(typeof(TProcessor));
        return this;
    }

    public NotificationProcessorsConfig Build()
    {
        return new NotificationProcessorsConfig(_eventTypeMap, _eventNotificationProcessorsMap);
    }
}

public class NotificationProcessorsConfig(
    IReadOnlyDictionary<string, Type> eventTypeMap,
    IReadOnlyDictionary<Type, List<Type>> eventNotificationProcessorsMap
)
{
    public Type? GetEventType(string routingKey)
    {
        eventTypeMap.TryGetValue(routingKey, out var eventType);
        return eventType;
    }

    public IReadOnlyList<Type> GetAllNotificationProcessors()
    {
        return eventNotificationProcessorsMap.Values.SelectMany(v => v).ToList();
    }

    public IReadOnlyList<Type> GetEventNotificationProcessors(Type eventType)
    {
        return eventNotificationProcessorsMap.GetValueOrDefault(eventType, []);
    }
}
