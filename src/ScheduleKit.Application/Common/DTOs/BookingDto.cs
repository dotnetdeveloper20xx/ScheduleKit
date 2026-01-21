namespace ScheduleKit.Application.Common.DTOs;

/// <summary>
/// Response DTO for a booking.
/// </summary>
public record BookingResponse
{
    public Guid Id { get; init; }
    public Guid EventTypeId { get; init; }
    public string EventTypeName { get; init; } = string.Empty;
    public Guid HostUserId { get; init; }
    public string GuestName { get; init; } = string.Empty;
    public string GuestEmail { get; init; } = string.Empty;
    public string? GuestPhone { get; init; }
    public string? GuestNotes { get; init; }
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public string GuestTimezone { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? CancellationReason { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public string? MeetingLink { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public List<BookingQuestionAnswerResponse> Responses { get; init; } = new();
}

/// <summary>
/// Response DTO for a question answer.
/// </summary>
public record BookingQuestionAnswerResponse
{
    public Guid QuestionId { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public string ResponseValue { get; init; } = string.Empty;
}

/// <summary>
/// Confirmation response after creating a booking.
/// </summary>
public record BookingConfirmationResponse
{
    public Guid BookingId { get; init; }
    public string EventTypeName { get; init; } = string.Empty;
    public string HostName { get; init; } = string.Empty;
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public string GuestTimezone { get; init; } = string.Empty;
    public string? MeetingLink { get; init; }
    public string CancellationLink { get; init; } = string.Empty;
    public string RescheduleLink { get; init; } = string.Empty;
}
