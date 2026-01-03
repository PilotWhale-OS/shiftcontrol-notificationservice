namespace NotificationService.Service;

public class TestService(ILogger<TestService> logger)
{
    public string GetMessage()
    {
        return "lol";
    }
}
