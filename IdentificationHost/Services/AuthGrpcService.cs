using IdentificationApplication.Services;
using IdentificationContracts.Grpc;
using Grpc.Core;
using IdentificationApplication.Contracts;

namespace IdentificationHost.Services;

public sealed class AuthGrpcService : AuthService.AuthServiceBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthGrpcService> _logger;

    public AuthGrpcService(IAuthenticationService authenticationService, ILogger<AuthGrpcService> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    public override async Task<IssueTokenResponse> IssueToken(
        IssueTokenRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("IssueToken request for user: {Username}", request.Username);

        var appRequest = new IdentificationApplication.Contracts.IssueTokenRequest(request.Username, request.Password);

        IdentificationApplication.Contracts.IssueTokenResponse result = await _authenticationService
            .IssueTokenAsync(appRequest, context.CancellationToken);

        return new IssueTokenResponse
        {
            Success = result.Success,
            Token = result.Token ?? string.Empty,
            ExpiresAtUnix = result.ExpiresAt.HasValue
                ? new DateTimeOffset(result.ExpiresAt.Value).ToUnixTimeSeconds()
                : 0,
            ErrorMessage = result.ErrorMessage ?? string.Empty,
        };
    }

    public override async Task<ValidateTokenResponse> ValidateToken(
        ValidateTokenRequest request,
        ServerCallContext context)
    {
        _logger.LogDebug("ValidateToken request");

        var appRequest = new IdentificationApplication.Contracts.ValidateTokenRequest(request.Token);

        IdentificationApplication.Contracts.ValidateTokenResponse result = await _authenticationService
            .ValidateTokenAsync(appRequest, context.CancellationToken);

        return new ValidateTokenResponse
        {
            Valid = result.Valid,
            UserId = result.UserId?.ToString() ?? string.Empty,
            Username = result.Username ?? string.Empty,
        };
    }

    public override async Task<RevokeTokenResponse> RevokeToken(
        RevokeTokenRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("RevokeToken request");

        bool success = await _authenticationService.RevokeTokenAsync(request.Token, context.CancellationToken);

        return new RevokeTokenResponse { Success = success };
    }
}