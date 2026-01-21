using System.Collections.Concurrent;
using System.Security.Cryptography;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// Mock OAuth service for demo purposes.
/// Simulates OAuth login flow without requiring actual OAuth provider credentials.
/// </summary>
public class MockOAuthService : IOAuthService
{
    private static readonly ConcurrentDictionary<string, (DateTime CreatedAt, string Provider)> _stateTokens = new();
    private static readonly TimeSpan StateTokenExpiry = TimeSpan.FromMinutes(10);

    public IReadOnlyList<string> SupportedProviders { get; } = new[]
    {
        "google",
        "microsoft",
        "github"
    };

    public string GetAuthorizationUrl(string provider, string redirectUri, string state)
    {
        // In a real implementation, this would redirect to the OAuth provider
        // For mock, we'll create a simulated OAuth URL that redirects back immediately
        var encodedRedirect = Uri.EscapeDataString(redirectUri);
        var encodedState = Uri.EscapeDataString(state);

        // Store the state for validation
        _stateTokens[state] = (DateTime.UtcNow, provider);

        // Return a mock OAuth URL that will be handled by our frontend
        // The frontend will redirect to the callback with a mock code
        return $"/oauth/mock/{provider}?redirect_uri={encodedRedirect}&state={encodedState}";
    }

    public async Task<OAuthUserInfo> ExchangeCodeAsync(
        string provider,
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken); // Simulate network delay

        // Validate the code format (in mock, code contains provider:email pattern)
        if (string.IsNullOrEmpty(code))
        {
            return OAuthUserInfo.Failure("Invalid authorization code");
        }

        // For mock purposes, the code contains the simulated user info
        // Format: mock_{provider}_{timestamp}
        if (!code.StartsWith("mock_"))
        {
            return OAuthUserInfo.Failure("Invalid mock authorization code");
        }

        // Generate mock user info based on provider
        var (email, name, avatarUrl) = GenerateMockUserInfo(provider);

        return new OAuthUserInfo
        {
            Success = true,
            ProviderId = Guid.NewGuid().ToString(),
            Email = email,
            Name = name,
            AvatarUrl = avatarUrl,
            Provider = provider,
            AccessToken = GenerateRandomToken(),
            RefreshToken = GenerateRandomToken(),
            TokenExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }

    public bool ValidateState(string state)
    {
        if (!_stateTokens.TryGetValue(state, out var tokenInfo))
        {
            return false;
        }

        // Check if state has expired
        if (DateTime.UtcNow - tokenInfo.CreatedAt > StateTokenExpiry)
        {
            _stateTokens.TryRemove(state, out _);
            return false;
        }

        // Remove used state (one-time use)
        _stateTokens.TryRemove(state, out _);
        return true;
    }

    public string GenerateState()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private static (string email, string name, string avatarUrl) GenerateMockUserInfo(string provider)
    {
        var timestamp = DateTime.UtcNow.Ticks % 10000;

        return provider.ToLowerInvariant() switch
        {
            "google" => (
                $"demo.user{timestamp}@gmail.com",
                "Demo Google User",
                $"https://ui-avatars.com/api/?name=Demo+Google&background=4285F4&color=fff"
            ),
            "microsoft" => (
                $"demo.user{timestamp}@outlook.com",
                "Demo Microsoft User",
                $"https://ui-avatars.com/api/?name=Demo+Microsoft&background=00A4EF&color=fff"
            ),
            "github" => (
                $"demouser{timestamp}@github.example.com",
                "DemoGitHubUser",
                $"https://ui-avatars.com/api/?name=Demo+GitHub&background=333&color=fff"
            ),
            _ => (
                $"demo.user{timestamp}@example.com",
                "Demo User",
                $"https://ui-avatars.com/api/?name=Demo+User&background=666&color=fff"
            )
        };
    }

    private static string GenerateRandomToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
