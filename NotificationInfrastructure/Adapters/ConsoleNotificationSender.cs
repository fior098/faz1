using NotificationDomain.Entities;
using NotificationDomain.Ports;
using Microsoft.Extensions.Logging;

namespace NotificationInfrastructure.Adapters;

public sealed class ConsoleNotificationSender : INotificationSender
{
    private readonly ILogger<ConsoleNotificationSender> _logger;

    public ConsoleNotificationSender(ILogger<ConsoleNotificationSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(NotificationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[NOTIFICATION] To: {UserId} | Type: {Type} | Title: {Title} | Body: {Body}",
            notification.UserId,
            notification.EventType,
            notification.Title,
            notification.Body);

        return Task.CompletedTask;
    }
}