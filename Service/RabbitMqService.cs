using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Classes.Dto;
using NotificationService.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationService.Hubs.Implementation;
using ShiftControl.Events;

namespace NotificationService.Service;

public class RabbitMqService : IHostedService
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IHubContext<TestHub, ITestHub> _hubContext;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger, IHubContext<TestHub, ITestHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
        var factory = new ConnectionFactory()
        {
            HostName = configuration.GetValue<string>("RabbitMQ:HostName"),
            Port = configuration.GetValue<int>("RabbitMQ:Port"),
            UserName = configuration.GetValue<string>("RabbitMQ:UserName"),
            Password = configuration.GetValue<string>("RabbitMQ:Password"),
            //DispatchConsumersAsync = true
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _channel.ExchangeDeclare("shiftcontrol", ExchangeType.Topic, durable: true);
        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: queueName,
            exchange: "shiftcontrol",
            routingKey: "#");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received message: {message}", message);



            if (ea.RoutingKey == "activity.created")
            {
                var activityEvent = ActivityEventSchema.FromJson(message);
            }




            // Assuming the message is a TestEventDto for now
            var testEvent = JsonSerializer.Deserialize<TestEventDto>(message);
            if (testEvent != null)
            {
                await _hubContext.Clients.All.TestEvent(testEvent);
            }
        };
        _channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);

        _logger.LogInformation("RabbitMQ consumer started.");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close();
        return Task.CompletedTask;
    }
}

