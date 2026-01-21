namespace ScheduleKit.Domain.Interfaces;

/// <summary>
/// Interface for external calendar synchronization services (Google Calendar, Outlook, etc.)
/// This is separate from ICalendarService in Application which handles ICS file generation.
/// </summary>
public interface IExternalCalendarService
{
    /// <summary>
    /// Gets the provider name (e.g., "Google Calendar", "Mock")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Creates a calendar event for a booking.
    /// </summary>
    Task<CalendarEventResult> CreateEventAsync(CalendarEventRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing calendar event.
    /// </summary>
    Task<CalendarEventResult> UpdateEventAsync(string externalEventId, CalendarEventRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a calendar event.
    /// </summary>
    Task<bool> DeleteEventAsync(string externalEventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets busy times for a user within a date range.
    /// </summary>
    Task<IReadOnlyList<BusyTimeSlot>> GetBusyTimesAsync(Guid userId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the service is connected/authenticated for a user.
    /// </summary>
    Task<bool> IsConnectedAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request to create or update a calendar event.
/// </summary>
public record CalendarEventRequest
{
    public Guid BookingId { get; init; }
    public Guid HostUserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public string? Location { get; init; }
    public string GuestEmail { get; init; } = string.Empty;
    public string GuestName { get; init; } = string.Empty;
    public string? MeetingUrl { get; init; }
}

/// <summary>
/// Result of a calendar event operation.
/// </summary>
public record CalendarEventResult
{
    public bool Success { get; init; }
    public string? ExternalEventId { get; init; }
    public string? CalendarLink { get; init; }
    public string? ErrorMessage { get; init; }

    public static CalendarEventResult Succeeded(string externalEventId, string? calendarLink = null)
        => new() { Success = true, ExternalEventId = externalEventId, CalendarLink = calendarLink };

    public static CalendarEventResult Failed(string errorMessage)
        => new() { Success = false, ErrorMessage = errorMessage };
}

/// <summary>
/// Represents a busy time slot from an external calendar.
/// </summary>
public record BusyTimeSlot
{
    public DateTime StartUtc { get; init; }
    public DateTime EndUtc { get; init; }
    public string? Title { get; init; }
}
