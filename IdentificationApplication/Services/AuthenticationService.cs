using System.Security.Cryptography;
using System.Text;
using IdentificationApplication.Contracts;
using IdentificationDomain.Entities;
using IdentificationDomain.Ports;

namespace IdentificationApplication.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly TimeSpan _tokenTtl;

    public AuthenticationService(
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        TimeSpan tokenTtl)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _tokenTtl = tokenTtl;
    }

    public async Task<IssueTokenResponse> IssueTokenAsync(
        IssueTokenRequest request,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null)
        {
            return new IssueTokenResponse(false, null, null, "User not found");
        }

        string passwordHash = HashPassword(request.Password);

        if (!string.Equals(user.PasswordHash, passwordHash, StringComparison.Ordinal))
        {
            return new IssueTokenResponse(false, null, null, "Invalid password");
        }

        string token = GenerateOpaqueToken();
        DateTime expiresAt = DateTime.UtcNow.Add(_tokenTtl);

        var session = new Session(token, user.Id, expiresAt, DateTime.UtcNow);
        await _tokenRepository.SaveSessionAsync(session, cancellationToken);

        return new IssueTokenResponse(true, token, expiresAt, null);
    }

    public async Task<ValidateTokenResponse> ValidateTokenAsync(
        ValidateTokenRequest request,
        CancellationToken cancellationToken)
    {
        Session? session = await _tokenRepository.GetSessionAsync(request.Token, cancellationToken);

        if (session is null || session.IsExpired)
        {
            return new ValidateTokenResponse(false, null, null);
        }

        User? user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);

        if (user is null)
        {
            return new ValidateTokenResponse(false, null, null);
        }

        return new ValidateTokenResponse(true, user.Id, user.Username);
    }

    public async Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken)
    {
        Session? session = await _tokenRepository.GetSessionAsync(token, cancellationToken);

        if (session is null)
        {
            return false;
        }

        await _tokenRepository.DeleteSessionAsync(token, cancellationToken);
        return true;
    }

    private static string GenerateOpaqueToken()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }

    private static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}