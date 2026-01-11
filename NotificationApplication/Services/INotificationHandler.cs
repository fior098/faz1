using NotificationApplication.Contracts;

namespace NotificationApplication.Services;

public interface INotificationHandler
{
    Task HandleMessageSentAsync(MessageSentEvent message, CancellationToken cancellationToken);
}