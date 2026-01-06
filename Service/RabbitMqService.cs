using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationService.Settings;

namespace NotificationService.Service;

public class RabbitMqService(
    IOptions<RabbitMqSettings> settings,
    ILogger<RabbitMqService> logger,
    EventProcessorService eventProcessor) : IHostedService
{
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = settings.Value.HostName,
            Port = settings.Value.Port,
            UserName = settings.Value.UserName,
            Password = settings.Value.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var queueResult = await _channel.QueueDeclareAsync(
            "notificationservice.queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await _channel.QueueBindAsync(queue: queueResult.QueueName,
            exchange: "shiftcontrol",
            routingKey: "shiftcontrol.#",
            cancellationToken: cancellationToken
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += eventProcessor.HandleEventAsync;
        await _channel.BasicConsumeAsync(queueResult.QueueName, false, consumer, cancellationToken);

        logger.LogInformation("RabbitMQ consumer started.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.CloseAsync(cancellationToken: cancellationToken);
        _connection?.CloseAsync(cancellationToken: cancellationToken);
        return Task.CompletedTask;
    }
}

