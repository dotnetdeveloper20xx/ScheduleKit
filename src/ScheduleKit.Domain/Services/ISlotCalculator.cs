using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Domain.Services;

/// <summary>
/// Service for calculating available time slots for booking.
/// </summary>
public interface ISlotCalculator
{
    /// <summary>
    /// Calculates available time slots for a specific date.
    /// </summary>
    /// <param name="eventType">The event type being booked.</param>
    /// <param name="availabilities">The host's weekly availability.</param>
    /// <param name="overrides">Any availability overrides for the date.</param>
    /// <param name="existingBookings">Existing bookings that block time.</param>
    /// <param name="date">The date to calculate slots for.</param>
    /// <param name="hostTimezone">The host's timezone (IANA format).</param>
    /// <returns>List of available time slots.</returns>
    List<CalculatedSlot> CalculateSlotsForDate(
        EventType eventType,
        List<Availability> availabilities,
        List<AvailabilityOverride> overrides,
        List<Booking> existingBookings,
        DateOnly date,
        string hostTimezone);

    /// <summary>
    /// Checks if a specific time slot is available.
    /// </summary>
    bool IsSlotAvailable(
        EventType eventType,
        List<Availability> availabilities,
        List<AvailabilityOverride> overrides,
        List<Booking> existingBookings,
        DateTime proposedStartUtc,
        string hostTimezone);
}

/// <summary>
/// Represents a calculated time slot.
/// </summary>
public record CalculatedSlot
{
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public bool IsAvailable { get; init; }
}
