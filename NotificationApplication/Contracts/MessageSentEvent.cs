namespace NotificationApplication.Contracts;

public sealed record MessageSentEvent(
    Guid MessageId,
    Guid ChatId,
    Guid SenderId,
    Guid RecipientId,
    string SenderName,
    string MessagePreview,
    DateTime SentAt);