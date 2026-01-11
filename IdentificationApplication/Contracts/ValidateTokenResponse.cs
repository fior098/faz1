namespace IdentificationApplication.Contracts;

public sealed record ValidateTokenResponse(bool Valid, Guid? UserId, string? Username);