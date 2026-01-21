namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// Settings for external integration services.
/// Controls whether mock or real implementations are used.
/// </summary>
public class ExternalIntegrationSettings
{
    public const string SectionName = "ExternalIntegrations";

    /// <summary>
    /// Whether to use mock implementations for all external services.
    /// Set to true for demo/development, false for production with real APIs.
    /// </summary>
    public bool UseMockServices { get; set; } = true;

    /// <summary>
    /// Calendar integration settings.
    /// </summary>
    public CalendarSettings Calendar { get; set; } = new();

    /// <summary>
    /// Video conferencing settings.
    /// </summary>
    public VideoConferenceSettings VideoConference { get; set; } = new();

    /// <summary>
    /// OAuth authentication settings.
    /// </summary>
    public OAuthSettings OAuth { get; set; } = new();
}

/// <summary>
/// Calendar integration settings.
/// </summary>
public class CalendarSettings
{
    /// <summary>
    /// Google Calendar API credentials.
    /// </summary>
    public GoogleCalendarSettings? Google { get; set; }

    /// <summary>
    /// Microsoft Outlook/Graph API credentials.
    /// </summary>
    public MicrosoftCalendarSettings? Microsoft { get; set; }
}

/// <summary>
/// Google Calendar API settings.
/// </summary>
public class GoogleCalendarSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

/// <summary>
/// Microsoft Calendar/Graph API settings.
/// </summary>
public class MicrosoftCalendarSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
}

/// <summary>
/// Video conferencing settings.
/// </summary>
public class VideoConferenceSettings
{
    /// <summary>
    /// Zoom API credentials.
    /// </summary>
    public ZoomSettings? Zoom { get; set; }

    /// <summary>
    /// Google Meet uses Google Calendar credentials.
    /// </summary>
    public bool GoogleMeetEnabled { get; set; }

    /// <summary>
    /// Microsoft Teams uses Microsoft Graph credentials.
    /// </summary>
    public bool MicrosoftTeamsEnabled { get; set; }
}

/// <summary>
/// Zoom API settings.
/// </summary>
public class ZoomSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
}

/// <summary>
/// OAuth authentication settings.
/// </summary>
public class OAuthSettings
{
    /// <summary>
    /// Google OAuth settings for social login.
    /// </summary>
    public GoogleOAuthSettings? Google { get; set; }

    /// <summary>
    /// Microsoft OAuth settings for social login.
    /// </summary>
    public MicrosoftOAuthSettings? Microsoft { get; set; }

    /// <summary>
    /// GitHub OAuth settings for social login.
    /// </summary>
    public GitHubOAuthSettings? GitHub { get; set; }
}

/// <summary>
/// Google OAuth settings.
/// </summary>
public class GoogleOAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

/// <summary>
/// Microsoft OAuth settings.
/// </summary>
public class MicrosoftOAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
}

/// <summary>
/// GitHub OAuth settings.
/// </summary>
public class GitHubOAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
