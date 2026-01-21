namespace ScheduleKit.Api.Models;

/// <summary>
/// Request model for updating user profile.
/// </summary>
public record UpdateProfileRequest
{
    /// <summary>
    /// User's display name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// User's public booking URL slug.
    /// </summary>
    public string? Slug { get; init; }
}

/// <summary>
/// Request model for updating user timezone.
/// </summary>
public record UpdateTimezoneRequest
{
    /// <summary>
    /// User's timezone (IANA format, e.g., "America/New_York").
    /// </summary>
    public string Timezone { get; init; } = string.Empty;
}

/// <summary>
/// Request model for updating email preferences.
/// </summary>
public record UpdateEmailPreferencesRequest
{
    /// <summary>
    /// Whether email notifications are enabled.
    /// </summary>
    public bool EmailNotificationsEnabled { get; init; }

    /// <summary>
    /// Whether reminder emails are enabled.
    /// </summary>
    public bool ReminderEmailsEnabled { get; init; }

    /// <summary>
    /// Hours before event to send reminder (1-168).
    /// </summary>
    public int ReminderHoursBefore { get; init; }
}

/// <summary>
/// Response model for full user profile.
/// </summary>
public record UserProfileResponse
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

    /// <summary>
    /// Whether email notifications are enabled.
    /// </summary>
    public bool EmailNotificationsEnabled { get; init; }

    /// <summary>
    /// Whether reminder emails are enabled.
    /// </summary>
    public bool ReminderEmailsEnabled { get; init; }

    /// <summary>
    /// Hours before event to send reminder.
    /// </summary>
    public int ReminderHoursBefore { get; init; }

    /// <summary>
    /// Whether the account is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Last login timestamp.
    /// </summary>
    public DateTime? LastLoginAt { get; init; }
}
