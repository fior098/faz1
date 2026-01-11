using IdentificationDomain.Entities;

namespace IdentificationDomain.Ports;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task CreateUserAsync(User user, CancellationToken cancellationToken);
}