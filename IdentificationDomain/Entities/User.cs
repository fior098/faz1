namespace IdentificationDomain.Entities;

public sealed class User
{
    public User(Guid id, string username, string passwordHash, DateTime createdAt)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public string Username { get; }

    public string PasswordHash { get; }

    public DateTime CreatedAt { get; }
}