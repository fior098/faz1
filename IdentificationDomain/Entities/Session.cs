namespace IdentificationDomain.Entities;

public sealed class Session
{
    public Session(string token, Guid userId, DateTime expiresAt, DateTime createdAt)
    {
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    public string Token { get; }

    public Guid UserId { get; }

    public DateTime ExpiresAt { get; }

    public DateTime CreatedAt { get; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}