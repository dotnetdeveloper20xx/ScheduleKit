namespace ScheduleKit.Domain.Interfaces;

/// <summary>
/// Interface for video conferencing services (Zoom, Google Meet, Microsoft Teams, etc.)
/// </summary>
public interface IVideoConferenceService
{
    /// <summary>
    /// Gets the provider name (e.g., "Zoom", "Google Meet", "Mock")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Gets the supported location types for this provider.
    /// </summary>
    IReadOnlyList<string> SupportedLocationTypes { get; }

    /// <summary>
    /// Creates a video conference meeting.
    /// </summary>
    Task<MeetingResult> CreateMeetingAsync(MeetingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing meeting.
    /// </summary>
    Task<MeetingResult> UpdateMeetingAsync(string externalMeetingId, MeetingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes/cancels a meeting.
    /// </summary>
    Task<bool> DeleteMeetingAsync(string externalMeetingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the service is connected/authenticated for a user.
    /// </summary>
    Task<bool> IsConnectedAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request to create or update a video conference meeting.
/// </summary>
public record MeetingRequest
{
    public Guid BookingId { get; init; }
    public Guid HostUserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public int DurationMinutes { get; init; }
    public string GuestEmail { get; init; } = string.Empty;
    public string GuestName { get; init; } = string.Empty;
    public string LocationType { get; init; } = string.Empty;
}

/// <summary>
/// Result of a meeting creation/update operation.
/// </summary>
public record MeetingResult
{
    public bool Success { get; init; }
    public string? ExternalMeetingId { get; init; }
    public string? JoinUrl { get; init; }
    public string? HostUrl { get; init; }
    public string? Password { get; init; }
    public string? DialInNumber { get; init; }
    public string? ErrorMessage { get; init; }

    public static MeetingResult Succeeded(string externalMeetingId, string joinUrl, string? hostUrl = null, string? password = null)
        => new() { Success = true, ExternalMeetingId = externalMeetingId, JoinUrl = joinUrl, HostUrl = hostUrl, Password = password };

    public static MeetingResult Failed(string errorMessage)
        => new() { Success = false, ErrorMessage = errorMessage };
}
