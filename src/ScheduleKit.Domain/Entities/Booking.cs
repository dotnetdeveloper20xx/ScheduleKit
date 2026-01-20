using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Events;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Domain.Entities;

/// <summary>
/// Status of a booking.
/// </summary>
public enum BookingStatus
{
    Confirmed,
    Cancelled,
    Completed,
    NoShow
}

/// <summary>
/// Represents a confirmed booking/appointment.
/// This is an aggregate root.
/// </summary>
public class Booking : BaseEntity, IAggregateRoot
{
    private readonly List<BookingQuestionResponse> _responses = new();

    public Guid EventTypeId { get; private set; }
    public Guid HostUserId { get; private set; }
    public string GuestName { get; private set; } = string.Empty;
    public string GuestEmail { get; private set; } = string.Empty;
    public string? GuestPhone { get; private set; }
    public string? GuestNotes { get; private set; }
    public DateTime StartTimeUtc { get; private set; }
    public DateTime EndTimeUtc { get; private set; }
    public string GuestTimezone { get; private set; } = string.Empty;
    public BookingStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? RescheduleToken { get; private set; }
    public string? MeetingLink { get; private set; }

    public IReadOnlyCollection<BookingQuestionResponse> Responses => _responses.AsReadOnly();

    // Navigation property
    public EventType? EventType { get; private set; }

    // For EF Core
    private Booking() { }

    private Booking(
        Guid eventTypeId,
        Guid hostUserId,
        GuestInfo guest,
        TimeSlot scheduledTime,
        string? guestNotes,
        string? meetingLink)
    {
        Id = Guid.NewGuid();
        EventTypeId = eventTypeId;
        HostUserId = hostUserId;
        GuestName = guest.Name;
        GuestEmail = guest.Email.Value;
        GuestPhone = guest.Phone;
        GuestTimezone = guest.Timezone;
        GuestNotes = guestNotes;
        StartTimeUtc = scheduledTime.StartTimeUtc;
        EndTimeUtc = scheduledTime.EndTimeUtc;
        Status = BookingStatus.Confirmed;
        MeetingLink = meetingLink;
        RescheduleToken = GenerateToken();
        SetCreatedAt();

        AddDomainEvent(new BookingCreatedEvent(Id, EventTypeId, HostUserId, GuestEmail, StartTimeUtc));
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    public static Result<Booking> Create(
        EventType eventType,
        GuestInfo guest,
        TimeSlot scheduledTime,
        string? guestNotes = null,
        string? meetingLink = null)
    {
        if (!eventType.IsActive)
            return Result.Failure<Booking>("This event type is not accepting bookings.");

        if (scheduledTime.StartTimeUtc <= DateTime.UtcNow)
            return Result.Failure<Booking>("Cannot book in the past.");

        if (guestNotes?.Length > 2000)
            return Result.Failure<Booking>("Guest notes are too long.");

        // Verify duration matches event type
        var expectedDuration = eventType.Duration.ToTimeSpan();
        if (Math.Abs((scheduledTime.Duration - expectedDuration).TotalMinutes) > 1)
            return Result.Failure<Booking>("Booking duration does not match event type.");

        return Result.Success(new Booking(
            eventType.Id,
            eventType.HostUserId,
            guest,
            scheduledTime,
            guestNotes?.Trim(),
            meetingLink));
    }

    /// <summary>
    /// Cancels the booking.
    /// </summary>
    public Result Cancel(string? reason, CancelledBy cancelledBy)
    {
        if (Status == BookingStatus.Cancelled)
            return Result.Failure("Booking is already cancelled.");

        if (Status == BookingStatus.Completed)
            return Result.Failure("Cannot cancel a completed booking.");

        if (StartTimeUtc <= DateTime.UtcNow)
            return Result.Failure("Cannot cancel past bookings.");

        if (reason?.Length > 500)
            return Result.Failure("Cancellation reason is too long.");

        Status = BookingStatus.Cancelled;
        CancellationReason = reason?.Trim();
        CancelledAtUtc = DateTime.UtcNow;
        SetUpdatedAt();

        AddDomainEvent(new BookingCancelledEvent(
            Id, HostUserId, GuestEmail, StartTimeUtc, cancelledBy, reason));

        return Result.Success();
    }

    /// <summary>
    /// Reschedules the booking to a new time.
    /// </summary>
    public Result Reschedule(TimeSlot newTime, Duration expectedDuration)
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure("Only confirmed bookings can be rescheduled.");

        if (newTime.StartTimeUtc <= DateTime.UtcNow)
            return Result.Failure("Cannot reschedule to a past time.");

        var expected = expectedDuration.ToTimeSpan();
        if (Math.Abs((newTime.Duration - expected).TotalMinutes) > 1)
            return Result.Failure("New time slot duration does not match event type.");

        var oldStartTime = StartTimeUtc;
        StartTimeUtc = newTime.StartTimeUtc;
        EndTimeUtc = newTime.EndTimeUtc;
        RescheduleToken = GenerateToken(); // Generate new token
        SetUpdatedAt();

        AddDomainEvent(new BookingRescheduledEvent(Id, oldStartTime, StartTimeUtc));

        return Result.Success();
    }

    /// <summary>
    /// Marks the booking as completed.
    /// </summary>
    public Result MarkCompleted()
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure("Only confirmed bookings can be marked as completed.");

        if (EndTimeUtc > DateTime.UtcNow)
            return Result.Failure("Cannot mark future bookings as completed.");

        Status = BookingStatus.Completed;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Marks the booking as a no-show.
    /// </summary>
    public Result MarkNoShow()
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure("Only confirmed bookings can be marked as no-show.");

        if (EndTimeUtc > DateTime.UtcNow)
            return Result.Failure("Cannot mark future bookings as no-show.");

        Status = BookingStatus.NoShow;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Adds a response to a booking question.
    /// </summary>
    public Result AddResponse(Guid questionId, string responseValue)
    {
        if (_responses.Any(r => r.QuestionId == questionId))
            return Result.Failure("Response for this question already exists.");

        var responseResult = BookingQuestionResponse.Create(Id, questionId, responseValue);
        if (responseResult.IsFailure)
            return Result.Failure(responseResult.Error);

        _responses.Add(responseResult.Value);
        return Result.Success();
    }

    /// <summary>
    /// Validates that the provided token matches the reschedule token.
    /// </summary>
    public bool ValidateRescheduleToken(string token)
    {
        return !string.IsNullOrEmpty(RescheduleToken) &&
               string.Equals(RescheduleToken, token, StringComparison.Ordinal);
    }

    /// <summary>
    /// Gets the time slot as a value object.
    /// </summary>
    public TimeSlot GetTimeSlot() => TimeSlot.FromStorage(StartTimeUtc, EndTimeUtc);

    /// <summary>
    /// Checks if this booking overlaps with a time slot.
    /// </summary>
    public bool OverlapsWith(TimeSlot slot)
    {
        return StartTimeUtc < slot.EndTimeUtc && EndTimeUtc > slot.StartTimeUtc;
    }

    private static string GenerateToken()
    {
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}
