namespace ScheduleKit.Application.Common.Interfaces;

/// <summary>
/// Interface for email sending service.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a booking confirmation email to the guest.
    /// </summary>
    Task SendBookingConfirmationAsync(BookingEmailData data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a booking cancellation email to the guest.
    /// </summary>
    Task SendBookingCancellationAsync(BookingEmailData data, string? reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a booking rescheduled email to the guest.
    /// </summary>
    Task SendBookingRescheduledAsync(BookingEmailData data, DateTime oldStartTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a booking reminder email to the guest.
    /// </summary>
    Task SendBookingReminderAsync(BookingEmailData data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a new booking notification email to the host.
    /// </summary>
    Task SendNewBookingNotificationToHostAsync(BookingEmailData data, string hostEmail, CancellationToken cancellationToken = default);
}

/// <summary>
/// Data required for booking-related emails.
/// </summary>
public record BookingEmailData
{
    public Guid BookingId { get; init; }
    public string EventTypeName { get; init; } = string.Empty;
    public string HostName { get; init; } = string.Empty;
    public string GuestName { get; init; } = string.Empty;
    public string GuestEmail { get; init; } = string.Empty;
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public string GuestTimezone { get; init; } = string.Empty;
    public string? MeetingLink { get; init; }
    public string? LocationDetails { get; init; }
    public string CancellationLink { get; init; } = string.Empty;
    public string RescheduleLink { get; init; } = string.Empty;
    public string? IcsAttachment { get; init; }
}
