namespace NotificationInfrastructure.Kafka;

public sealed class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";

    public string GroupId { get; set; } = "notification-service";

    public string MessageSentTopic { get; set; } = "chat.message.sent.v1";

    public string InferenceResultTopic { get; set; } = "ai.inference.raw_result.v1";
}