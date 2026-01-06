namespace NotificationService.Classes;

public interface INotificationProcessor
{
    Task<object> BuildMessage(object eventData);
    Task<IEnumerable<string>> GetRecipients(object eventData);
}

public interface INotificationProcessor<TEvent> where TEvent : class
{
    public Task<EmailNotification?> BuildEmail(TEvent eventData);
    public Task<PushNotification?> BuildPush(TEvent eventData);
}
