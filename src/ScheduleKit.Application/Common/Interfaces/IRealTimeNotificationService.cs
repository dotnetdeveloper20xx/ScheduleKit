namespace ScheduleKit.Application.Common.Interfaces;

/// <summary>
/// Interface for real-time notification service (SignalR).
/// </summary>
public interface IRealTimeNotificationService
{
    /// <summary>
    /// Notifies clients that a slot has been booked.
    /// </summary>
    Task NotifySlotBookedAsync(Guid eventTypeId, DateTime startTimeUtc, CancellationToken ct = default);

    /// <summary>
    /// Notifies clients that a slot has been released (cancellation).
    /// </summary>
    Task NotifySlotReleasedAsync(Guid eventTypeId, DateTime startTimeUtc, CancellationToken ct = default);

    /// <summary>
    /// Notifies hosts that a new booking has been created.
    /// </summary>
    Task NotifyBookingCreatedAsync(BookingCreatedNotification notification, CancellationToken ct = default);

    /// <summary>
    /// Notifies hosts that a booking has been cancelled.
    /// </summary>
    Task NotifyBookingCancelledAsync(Guid hostUserId, Guid bookingId, string? reason, CancellationToken ct = default);

    /// <summary>
    /// Notifies hosts and event watchers about a rescheduled booking.
    /// </summary>
    Task NotifyBookingRescheduledAsync(Guid hostUserId, Guid eventTypeId, Guid bookingId,
        DateTime oldStartTimeUtc, DateTime newStartTimeUtc, CancellationToken ct = default);
}

/// <summary>
/// Notification payload for new bookings.
/// </summary>
public record BookingCreatedNotification
{
    public Guid HostUserId { get; init; }
    public Guid BookingId { get; init; }
    public Guid EventTypeId { get; init; }
    public string EventTypeName { get; init; } = string.Empty;
    public string GuestName { get; init; } = string.Empty;
    public string GuestEmail { get; init; } = string.Empty;
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public string GuestTimezone { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
