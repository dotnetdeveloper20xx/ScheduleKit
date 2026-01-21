using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Domain.Entities;

/// <summary>
/// Represents a host user in the system.
/// This is an aggregate root.
/// </summary>
public class User : BaseEntity, IAggregateRoot
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Slug { get; private set; }
    public string Timezone { get; private set; } = "UTC";
    public bool EmailNotificationsEnabled { get; private set; } = true;
    public bool ReminderEmailsEnabled { get; private set; } = true;
    public int ReminderHoursBefore { get; private set; } = 24;
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAtUtc { get; private set; }

    // For EF Core
    private User() { }

    private User(string email, string passwordHash, string name, string? slug)
    {
        Id = Guid.NewGuid();
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        Name = name;
        Slug = slug?.ToLowerInvariant() ?? GenerateSlugFromName(name);
        SetCreatedAt();
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    public static Result<User> Create(string email, string passwordHash, string name, string? slug = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<User>("Email is required.");

        var emailValidation = ValueObjects.Email.Create(email);
        if (emailValidation.IsFailure)
            return Result.Failure<User>(emailValidation.Error);

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure<User>("Password hash is required.");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<User>("Name is required.");

        if (name.Length > 200)
            return Result.Failure<User>("Name is too long.");

        if (!string.IsNullOrEmpty(slug))
        {
            if (slug.Length < 3 || slug.Length > 50)
                return Result.Failure<User>("Slug must be between 3 and 50 characters.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(slug, @"^[a-z0-9-]+$"))
                return Result.Failure<User>("Slug can only contain lowercase letters, numbers, and hyphens.");
        }

        return Result.Success(new User(email, passwordHash, name, slug));
    }

    /// <summary>
    /// Updates the user's profile.
    /// </summary>
    public Result UpdateProfile(string name, string? slug = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Name is required.");

        if (name.Length > 200)
            return Result.Failure("Name is too long.");

        if (!string.IsNullOrEmpty(slug))
        {
            if (slug.Length < 3 || slug.Length > 50)
                return Result.Failure("Slug must be between 3 and 50 characters.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(slug, @"^[a-z0-9-]+$"))
                return Result.Failure("Slug can only contain lowercase letters, numbers, and hyphens.");

            Slug = slug.ToLowerInvariant();
        }

        Name = name.Trim();
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Updates the user's timezone.
    /// </summary>
    public Result UpdateTimezone(string timezone)
    {
        if (string.IsNullOrWhiteSpace(timezone))
            return Result.Failure("Timezone is required.");

        // Validate timezone
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            // Try IANA format
            if (!TryConvertIanaToWindows(timezone, out _))
                return Result.Failure($"Invalid timezone: {timezone}");
        }

        Timezone = timezone;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Updates email notification preferences.
    /// </summary>
    public Result UpdateEmailPreferences(
        bool emailNotificationsEnabled,
        bool reminderEmailsEnabled,
        int reminderHoursBefore)
    {
        if (reminderHoursBefore < 1 || reminderHoursBefore > 168)
            return Result.Failure("Reminder hours must be between 1 and 168 (1 week).");

        EmailNotificationsEnabled = emailNotificationsEnabled;
        ReminderEmailsEnabled = reminderEmailsEnabled;
        ReminderHoursBefore = reminderHoursBefore;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Updates the password hash.
    /// </summary>
    public Result UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            return Result.Failure("Password hash is required.");

        PasswordHash = newPasswordHash;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Records a login.
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAtUtc = DateTime.UtcNow;
        SetUpdatedAt();
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    /// <summary>
    /// Reactivates the user account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    private static string GenerateSlugFromName(string name)
    {
        var slug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace("'", "");

        // Remove any characters that aren't alphanumeric or hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9-]", "");

        // Remove multiple consecutive hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");

        // Trim hyphens from start and end
        slug = slug.Trim('-');

        // Add random suffix for uniqueness
        var random = new Random();
        slug += $"-{random.Next(1000, 9999)}";

        return slug;
    }

    private static bool TryConvertIanaToWindows(string ianaTimezone, out string? windowsTimezone)
    {
        // Common IANA to Windows timezone mappings
        var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "America/New_York", "Eastern Standard Time" },
            { "America/Chicago", "Central Standard Time" },
            { "America/Denver", "Mountain Standard Time" },
            { "America/Los_Angeles", "Pacific Standard Time" },
            { "Europe/London", "GMT Standard Time" },
            { "Europe/Paris", "Romance Standard Time" },
            { "Europe/Berlin", "W. Europe Standard Time" },
            { "Asia/Tokyo", "Tokyo Standard Time" },
            { "Asia/Shanghai", "China Standard Time" },
            { "Australia/Sydney", "AUS Eastern Standard Time" },
            { "UTC", "UTC" },
        };

        return mappings.TryGetValue(ianaTimezone, out windowsTimezone);
    }
}
