namespace IdentificationApplication.Contracts;

public sealed record IssueTokenResponse(
    bool Success,
    string? Token,
    DateTime? ExpiresAt,
    string? ErrorMessage);