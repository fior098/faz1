using NotificationApplication.Contracts;
using NotificationDomain.Entities;
using NotificationDomain.Ports;

namespace NotificationApplication.Services;

public sealed class NotificationHandler : INotificationHandler
{
    private readonly INotificationSender _notificationSender;

    public NotificationHandler(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task HandleMessageSentAsync(MessageSentEvent message, CancellationToken cancellationToken)
    {
        var notification = new NotificationEvent(
            id: Guid.NewGuid(),
            eventType: "message.sent",
            userId: message.RecipientId,
            title: string.Format("New message from {0}", message.SenderName),
            body: TruncateMessage(message.MessagePreview, 100),
            createdAt: DateTime.UtcNow);

        await _notificationSender.SendAsync(notification, cancellationToken);
    }

    private static string TruncateMessage(string message, int maxLength)
    {
        if (string.IsNullOrEmpty(message))
        {
            return string.Empty;
        }

        if (message.Length <= maxLength)
        {
            return message;
        }

        return message.Substring(0, maxLength - 3) + "...";
    }
}