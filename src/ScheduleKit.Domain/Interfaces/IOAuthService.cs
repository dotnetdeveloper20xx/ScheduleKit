namespace ScheduleKit.Domain.Interfaces;

/// <summary>
/// Interface for OAuth authentication providers.
/// </summary>
public interface IOAuthService
{
    /// <summary>
    /// Gets the list of supported OAuth providers.
    /// </summary>
    IReadOnlyList<string> SupportedProviders { get; }

    /// <summary>
    /// Generates the OAuth authorization URL for a provider.
    /// </summary>
    /// <param name="provider">OAuth provider name (google, microsoft, github).</param>
    /// <param name="redirectUri">The callback URL after authorization.</param>
    /// <param name="state">CSRF protection state parameter.</param>
    /// <returns>The authorization URL to redirect the user to.</returns>
    string GetAuthorizationUrl(string provider, string redirectUri, string state);

    /// <summary>
    /// Exchanges an authorization code for user information.
    /// </summary>
    /// <param name="provider">OAuth provider name.</param>
    /// <param name="code">The authorization code from the callback.</param>
    /// <param name="redirectUri">The callback URL used in the initial request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OAuth user information.</returns>
    Task<OAuthUserInfo> ExchangeCodeAsync(
        string provider,
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates an OAuth state parameter.
    /// </summary>
    bool ValidateState(string state);

    /// <summary>
    /// Generates a new OAuth state parameter.
    /// </summary>
    string GenerateState();
}

/// <summary>
/// User information returned from OAuth provider.
/// </summary>
public class OAuthUserInfo
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public string? ProviderId { get; init; }
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? AvatarUrl { get; init; }
    public string? Provider { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? TokenExpiresAt { get; init; }

    public static OAuthUserInfo Failure(string error) => new() { Success = false, Error = error };
}
