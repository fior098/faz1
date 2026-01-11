using NotificationApplication.Contracts;
using NotificationApplication.Services;
using NotificationDomain.Entities;
using NotificationDomain.Ports;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace NotificationTests;

public sealed class NotificationHandlerTests
{
    private readonly INotificationSender _notificationSender;
    private readonly NotificationHandler _handler;

    public NotificationHandlerTests()
    {
        _notificationSender = Substitute.For<INotificationSender>();
        _handler = new NotificationHandler(_notificationSender);
    }

    [Fact]
    public async Task HandleMessageSentSendsNotificationToRecipient()
    {
        var evt = new MessageSentEvent(
            MessageId: Guid.NewGuid(),
            ChatId: Guid.NewGuid(),
            SenderId: Guid.NewGuid(),
            RecipientId: Guid.NewGuid(),
            SenderName: "Alice",
            MessagePreview: "Hello, how are you?",
            SentAt: DateTime.UtcNow);

        await _handler.HandleMessageSentAsync(evt, CancellationToken.None);

        await _notificationSender.Received(1)
            .SendAsync(Arg.Is<NotificationEvent>(n =>
                n.UserId == evt.RecipientId &&
                n.Title.Contains("Alice") &&
                n.Body == "Hello, how are you?"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleMessageSentTruncatesLongMessages()
    {
        string longMessage = new string('A', 200);

        var evt = new MessageSentEvent(
            MessageId: Guid.NewGuid(),
            ChatId: Guid.NewGuid(),
            SenderId: Guid.NewGuid(),
            RecipientId: Guid.NewGuid(),
            SenderName: "Bob",
            MessagePreview: longMessage,
            SentAt: DateTime.UtcNow);

        await _handler.HandleMessageSentAsync(evt, CancellationToken.None);

        await _notificationSender.Received(1)
            .SendAsync(Arg.Is<NotificationEvent>(n =>
                n.Body.Length == 100 &&
                n.Body.EndsWith("...")), Arg.Any<CancellationToken>());
    }
}