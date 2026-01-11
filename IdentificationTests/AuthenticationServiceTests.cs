using System.Security.Cryptography;
using System.Text;
using IdentificationApplication.Contracts;
using IdentificationApplication.Services;
using IdentificationDomain.Entities;
using IdentificationDomain.Ports;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace IdentificationTests;

public sealed class AuthenticationServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _tokenRepository = Substitute.For<ITokenRepository>();
        _service = new AuthenticationService(_userRepository, _tokenRepository, TimeSpan.FromHours(1));
    }

    [Fact]
    public async Task IssueTokenWithValidCredentialsReturnsToken()
    {
        var user = new User(
            Guid.NewGuid(),
            "testuser",
            HashPassword("password123"),
            DateTime.UtcNow);

        _userRepository.GetByUsernameAsync("testuser", Arg.Any<CancellationToken>())
            .Returns(user);

        var request = new IssueTokenRequest("testuser", "password123");

        IssueTokenResponse result = await _service.IssueTokenAsync(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task IssueTokenWithInvalidPasswordReturnsError()
    {
        var user = new User(
            Guid.NewGuid(),
            "testuser",
            HashPassword("password123"),
            DateTime.UtcNow);

        _userRepository.GetByUsernameAsync("testuser", Arg.Any<CancellationToken>())
            .Returns(user);

        var request = new IssueTokenRequest("testuser", "wrongpassword");

        IssueTokenResponse result = await _service.IssueTokenAsync(request, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid password");
    }

    [Fact]
    public async Task IssueTokenWithNonExistentUserReturnsError()
    {
        _userRepository.GetByUsernameAsync("unknown", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var request = new IssueTokenRequest("unknown", "password");

        IssueTokenResponse result = await _service.IssueTokenAsync(request, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("User not found");
    }

    [Fact]
    public async Task ValidateTokenWithValidTokenReturnsValid()
    {
        Guid userId = Guid.NewGuid();
        var session = new Session("token123", userId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow);
        var user = new User(userId, "testuser", "hash", DateTime.UtcNow);

        _tokenRepository.GetSessionAsync("token123", Arg.Any<CancellationToken>())
            .Returns(session);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        var request = new ValidateTokenRequest("token123");

        ValidateTokenResponse result = await _service.ValidateTokenAsync(request, CancellationToken.None);

        result.Valid.Should().BeTrue();
        result.UserId.Should().Be(userId);
        result.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task ValidateTokenWithExpiredTokenReturnsInvalid()
    {
        var session = new Session("token123", Guid.NewGuid(), DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);

        _tokenRepository.GetSessionAsync("token123", Arg.Any<CancellationToken>())
            .Returns(session);

        var request = new ValidateTokenRequest("token123");

        ValidateTokenResponse result = await _service.ValidateTokenAsync(request, CancellationToken.None);

        result.Valid.Should().BeFalse();
    }

    private static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}