using NotificationDomain.Entities;

namespace NotificationDomain.Ports;

public interface INotificationSender
{
    Task SendAsync(NotificationEvent notification, CancellationToken cancellationToken);
}