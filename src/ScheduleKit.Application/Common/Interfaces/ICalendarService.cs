namespace ScheduleKit.Application.Common.Interfaces;

/// <summary>
/// Interface for calendar file generation.
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Generates an ICS calendar file content for a booking.
    /// </summary>
    string GenerateIcsForBooking(CalendarEventData data);

    /// <summary>
    /// Generates an ICS calendar file content for a cancellation.
    /// </summary>
    string GenerateIcsCancellation(CalendarEventData data);
}

/// <summary>
/// Data required to generate a calendar event.
/// </summary>
public record CalendarEventData
{
    public Guid BookingId { get; init; }
    public string EventTitle { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public string? Location { get; init; }
    public string OrganizerName { get; init; } = string.Empty;
    public string OrganizerEmail { get; init; } = string.Empty;
    public string AttendeeName { get; init; } = string.Empty;
    public string AttendeeEmail { get; init; } = string.Empty;
    public int SequenceNumber { get; init; } = 0;
}
