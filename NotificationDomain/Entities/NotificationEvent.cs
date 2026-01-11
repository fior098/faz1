namespace NotificationDomain.Entities;

public sealed class NotificationEvent
{
    public NotificationEvent(
        Guid id,
        string eventType,
        Guid userId,
        string title,
        string body,
        DateTime createdAt)
    {
        Id = id;
        EventType = eventType;
        UserId = userId;
        Title = title;
        Body = body;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public string EventType { get; }

    public Guid UserId { get; }

    public string Title { get; }

    public string Body { get; }

    public DateTime CreatedAt { get; }
}