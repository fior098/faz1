using System.Text.Json;
using IdentificationDomain.Entities;
using IdentificationDomain.Ports;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace IdentificationInfrastructure.Persistence;

public sealed class RedisTokenRepository : ITokenRepository
{
    private const string TokenKeyPrefix = "session:";
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisTokenRepository> _logger;

    public RedisTokenRepository(IConnectionMultiplexer redis, ILogger<RedisTokenRepository> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task SaveSessionAsync(Session session, CancellationToken cancellationToken)
    {
        IDatabase db = _redis.GetDatabase();
        string key = TokenKeyPrefix + session.Token;

        var data = new SessionData(
            session.UserId.ToString(),
            session.ExpiresAt.ToString("O"),
            session.CreatedAt.ToString("O"));

        string json = JsonSerializer.Serialize(data);
        TimeSpan ttl = session.ExpiresAt - DateTime.UtcNow;

        if (ttl > TimeSpan.Zero)
        {
            await db.StringSetAsync(key, json, ttl);
            _logger.LogInformation("Session saved for user {UserId}", session.UserId);
        }
    }

    public async Task<Session?> GetSessionAsync(string token, CancellationToken cancellationToken)
    {
        IDatabase db = _redis.GetDatabase();
        string key = TokenKeyPrefix + token;

        RedisValue value = await db.StringGetAsync(key);

        if (value.IsNullOrEmpty)
        {
            return null;
        }

        SessionData? data = JsonSerializer.Deserialize<SessionData>(value.ToString());

        if (data is null)
        {
            return null;
        }

        return new Session(
            token,
            Guid.Parse(data.UserId),
            DateTime.Parse(data.ExpiresAt),
            DateTime.Parse(data.CreatedAt));
    }

    public async Task DeleteSessionAsync(string token, CancellationToken cancellationToken)
    {
        IDatabase db = _redis.GetDatabase();
        string key = TokenKeyPrefix + token;
        await db.KeyDeleteAsync(key);
        _logger.LogInformation("Session deleted: {Token}", token.Substring(0, Math.Min(8, token.Length)));
    }

    private sealed record SessionData(string UserId, string ExpiresAt, string CreatedAt);
}