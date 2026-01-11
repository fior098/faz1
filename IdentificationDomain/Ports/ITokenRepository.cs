using IdentificationDomain.Entities;

namespace IdentificationDomain.Ports;

public interface ITokenRepository
{
    Task SaveSessionAsync(Session session, CancellationToken cancellationToken);

    Task<Session?> GetSessionAsync(string token, CancellationToken cancellationToken);

    Task DeleteSessionAsync(string token, CancellationToken cancellationToken);
}