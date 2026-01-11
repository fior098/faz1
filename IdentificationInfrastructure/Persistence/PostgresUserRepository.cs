using IdentificationDomain.Entities;
using IdentificationDomain.Ports;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace IdentificationInfrastructure.Persistence;

public sealed class PostgresUserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly ILogger<PostgresUserRepository> _logger;

    public PostgresUserRepository(string connectionString, ILogger<PostgresUserRepository> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        const string sql = "SELECT id, username, password_hash, created_at FROM users WHERE username = @username";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new User(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDateTime(3));
        }

        return null;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = "SELECT id, username, password_hash, created_at FROM users WHERE id = @id";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new User(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDateTime(3));
        }

        return null;
    }

    public async Task CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO users (id, username, password_hash, created_at)
            VALUES (@id, @username, @password_hash, @created_at)";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("@created_at", user.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
        _logger.LogInformation("User created: {Username}", user.Username);
    }
}