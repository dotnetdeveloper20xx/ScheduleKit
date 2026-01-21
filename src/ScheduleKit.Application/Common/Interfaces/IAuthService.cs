namespace ScheduleKit.Application.Common.Interfaces;

/// <summary>
/// Interface for authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Hashes a password.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    bool VerifyPassword(string password, string hash);

    /// <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    string GenerateToken(Guid userId, string email, string name);

    /// <summary>
    /// Generates a refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and returns the user ID if valid.
    /// </summary>
    Guid? ValidateToken(string token);
}

/// <summary>
/// Authentication result.
/// </summary>
public record AuthResult
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public UserInfo? User { get; init; }
    public string? Error { get; init; }

    public static AuthResult Succeeded(string accessToken, string refreshToken, DateTime expiresAt, UserInfo user)
        => new() { Success = true, AccessToken = accessToken, RefreshToken = refreshToken, ExpiresAt = expiresAt, User = user };

    public static AuthResult Failed(string error)
        => new() { Success = false, Error = error };
}

/// <summary>
/// User information included in auth responses.
/// </summary>
public record UserInfo
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Slug { get; init; }
    public string Timezone { get; init; } = "UTC";
}
