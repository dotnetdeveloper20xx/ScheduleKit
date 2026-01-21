namespace ScheduleKit.Api.Models;

/// <summary>
/// Request model for creating a public booking.
/// </summary>
public record CreatePublicBookingRequest
{
    /// <summary>
    /// The event type ID.
    /// </summary>
    public Guid EventTypeId { get; init; }

    /// <summary>
    /// Guest's name.
    /// </summary>
    public string GuestName { get; init; } = string.Empty;

    /// <summary>
    /// Guest's email address.
    /// </summary>
    public string GuestEmail { get; init; } = string.Empty;

    /// <summary>
    /// Guest's phone number (optional).
    /// </summary>
    public string? GuestPhone { get; init; }

    /// <summary>
    /// Additional notes from the guest (optional).
    /// </summary>
    public string? GuestNotes { get; init; }

    /// <summary>
    /// The selected start time in UTC (ISO 8601 format).
    /// </summary>
    public DateTime StartTimeUtc { get; init; }

    /// <summary>
    /// Guest's timezone (IANA format, e.g., "America/New_York").
    /// </summary>
    public string GuestTimezone { get; init; } = string.Empty;

    /// <summary>
    /// Responses to custom booking questions.
    /// </summary>
    public List<QuestionResponseRequest>? QuestionResponses { get; init; }
}

/// <summary>
/// Request model for a question response.
/// </summary>
public record QuestionResponseRequest
{
    /// <summary>
    /// The question ID.
    /// </summary>
    public Guid QuestionId { get; init; }

    /// <summary>
    /// The response value.
    /// </summary>
    public string ResponseValue { get; init; } = string.Empty;
}

/// <summary>
/// Request model for cancelling a booking.
/// </summary>
public record CancelBookingRequest
{
    /// <summary>
    /// Optional reason for cancellation.
    /// </summary>
    public string? Reason { get; init; }
}

/// <summary>
/// Request model for rescheduling a booking.
/// </summary>
public record RescheduleBookingRequest
{
    /// <summary>
    /// The new start time in UTC.
    /// </summary>
    public DateTime NewStartTimeUtc { get; init; }
}
