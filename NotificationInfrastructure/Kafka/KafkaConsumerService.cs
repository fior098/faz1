using System.Text.Json;
using NotificationApplication.Contracts;
using NotificationApplication.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NotificationInfrastructure.Kafka;

public sealed class KafkaConsumerService : BackgroundService
{
    private readonly INotificationHandler _notificationHandler;
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(
        INotificationHandler notificationHandler,
        IOptions<KafkaSettings> settings,
        ILogger<KafkaConsumerService> logger)
    {
        _notificationHandler = notificationHandler;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
        };

        using IConsumer<Ignore, string> consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        string[] topics = { _settings.MessageSentTopic, _settings.InferenceResultTopic };
        consumer.Subscribe(topics);

        _logger.LogInformation("Kafka consumer started. Subscribed to: {Topics}", string.Join(", ", topics));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ConsumeResult<Ignore, string> result = consumer.Consume(stoppingToken);

                _logger.LogDebug("Received message from topic: {Topic}", result.Topic);

                await ProcessMessageAsync(result.Topic, result.Message.Value, stoppingToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Error consuming message");
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        consumer.Close();
        _logger.LogInformation("Kafka consumer stopped");
    }

    private async Task ProcessMessageAsync(string topic, string message, CancellationToken cancellationToken)
    {
        if (string.Equals(topic, _settings.MessageSentTopic, StringComparison.Ordinal) ||
            string.Equals(topic, _settings.InferenceResultTopic, StringComparison.Ordinal))
        {
            MessageSentEvent? evt = JsonSerializer.Deserialize<MessageSentEvent>(message);

            if (evt is not null)
            {
                await _notificationHandler.HandleMessageSentAsync(evt, cancellationToken);
            }
        }
    }
}