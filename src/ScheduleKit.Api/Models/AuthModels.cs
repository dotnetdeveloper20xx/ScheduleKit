namespace ScheduleKit.Api.Models;

/// <summary>
/// Request model for user registration.
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// User's display name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Optional custom slug/username for public booking URL.
    /// </summary>
    public string? Slug { get; init; }
}

/// <summary>
/// Request model for user login.
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Response model for successful authentication.
/// </summary>
public record AuthResponse
{
    /// <summary>
    /// JWT access token for API authentication.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens.
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Access token expiration time in UTC.
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Authenticated user's profile.
    /// </summary>
    public UserResponse User { get; init; } = null!;
}

/// <summary>
/// Response model for user profile.
/// </summary>
public record UserResponse
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's display name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// User's public booking URL slug.
    /// </summary>
    public string? Slug { get; init; }

    /// <summary>
    /// User's timezone (IANA format).
    /// </summary>
    public string Timezone { get; init; } = "UTC";
}

/// <summary>
/// Response model for available OAuth providers.
/// </summary>
public record OAuthProvidersResponse
{
    /// <summary>
    /// List of available OAuth providers.
    /// </summary>
    public List<OAuthProviderInfo> Providers { get; init; } = new();
}

/// <summary>
/// Information about an OAuth provider.
/// </summary>
public record OAuthProviderInfo
{
    /// <summary>
    /// Provider identifier (e.g., "google", "microsoft").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Human-readable display name.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Icon class for UI rendering.
    /// </summary>
    public string IconClass { get; init; } = string.Empty;
}

/// <summary>
/// Response model for OAuth authorization initiation.
/// </summary>
public record OAuthAuthorizeResponse
{
    /// <summary>
    /// URL to redirect the user for OAuth authorization.
    /// </summary>
    public string AuthorizationUrl { get; init; } = string.Empty;

    /// <summary>
    /// State parameter for CSRF protection.
    /// </summary>
    public string State { get; init; } = string.Empty;
}

/// <summary>
/// Request model for OAuth callback.
/// </summary>
public record OAuthCallbackRequest
{
    /// <summary>
    /// OAuth provider name.
    /// </summary>
    public string Provider { get; init; } = string.Empty;

    /// <summary>
    /// Authorization code from OAuth provider.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// State parameter for CSRF validation.
    /// </summary>
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// Redirect URI used in the authorization request.
    /// </summary>
    public string RedirectUri { get; init; } = string.Empty;
}
