using System.Text;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotificationService.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationService.Hubs.Implementation;
using NotificationService.Converters;
using ShiftControl.Events;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NotificationService.Service;

public class RabbitMqService : IHostedService
{
    private readonly ILogger<RabbitMqService> _logger;
    private IConnection _connection;
    private readonly IConfiguration _configuration;
    private IChannel _channel;

    public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration.GetValue<string>("RabbitMQ:HostName"),
            Port = _configuration.GetValue<int>("RabbitMQ:Port"),
            UserName = _configuration.GetValue<string>("RabbitMQ:UserName"),
            Password = _configuration.GetValue<string>("RabbitMQ:Password"),
            //DispatchConsumersAsync = true
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        //await _channel.ExchangeDeclareAsync("shiftcontrol", ExchangeType.Topic, durable: true);
        var queueResult = await _channel.QueueDeclareAsync("notificationservice.queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(queue: queueResult.QueueName,
            exchange: "shiftcontrol",
            routingKey: "shiftcontrol.#");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            if (ea.RoutingKey == "shiftcontrol.activity.created")
            {
                var activityEvent = JsonConvert.DeserializeObject<ActivityEvent>(message, new JsonSerializerSettings
                {
                    Converters = { new UnixTimestampConverter() }
                });
                _logger.LogInformation("Deserialized ActivityEvent: {activityEvent}", activityEvent);
            }
        };
        await _channel.BasicConsumeAsync(queueResult.QueueName, false, consumer);

        _logger.LogInformation("RabbitMQ consumer started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}

