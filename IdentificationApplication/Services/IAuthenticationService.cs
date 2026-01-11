using IdentificationApplication.Contracts;

namespace IdentificationApplication.Services;

public interface IAuthenticationService
{
    Task<IssueTokenResponse> IssueTokenAsync(IssueTokenRequest request, CancellationToken cancellationToken);

    Task<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest request, CancellationToken cancellationToken);

    Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken);
}