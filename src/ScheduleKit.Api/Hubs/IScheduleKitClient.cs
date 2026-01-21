namespace ScheduleKit.Api.Hubs;

/// <summary>
/// SignalR client interface for real-time updates.
/// </summary>
public interface IScheduleKitClient
{
    /// <summary>
    /// Notifies clients that a slot has been booked.
    /// </summary>
    Task SlotBooked(Guid eventTypeId, DateTime startTimeUtc);

    /// <summary>
    /// Notifies clients that a slot has been released (cancellation).
    /// </summary>
    Task SlotReleased(Guid eventTypeId, DateTime startTimeUtc);

    /// <summary>
    /// Notifies hosts that a new booking has been created.
    /// </summary>
    Task BookingCreated(BookingNotification booking);

    /// <summary>
    /// Notifies hosts that a booking has been cancelled.
    /// </summary>
    Task BookingCancelled(Guid bookingId, string? reason);

    /// <summary>
    /// Notifies hosts that a booking has been rescheduled.
    /// </summary>
    Task BookingRescheduled(Guid bookingId, DateTime oldStartTimeUtc, DateTime newStartTimeUtc);
}

/// <summary>
/// Notification payload for new bookings.
/// </summary>
public record BookingNotification
{
    public Guid Id { get; init; }
    public Guid EventTypeId { get; init; }
    public string EventTypeName { get; init; } = string.Empty;
    public string GuestName { get; init; } = string.Empty;
    public string GuestEmail { get; init; } = string.Empty;
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public string GuestTimezone { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
