namespace ScheduleKit.Application.Common.DTOs;

/// <summary>
/// Response DTO for EventType.
/// </summary>
public record EventTypeResponse
{
    public Guid Id { get; init; }
    public Guid HostUserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DurationMinutes { get; init; }
    public int BufferBeforeMinutes { get; init; }
    public int BufferAfterMinutes { get; init; }
    public string LocationType { get; init; } = string.Empty;
    public string? LocationDetails { get; init; }
    public string? LocationDisplayName { get; init; }
    public bool IsActive { get; init; }
    public string? Color { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public List<BookingQuestionResponse> Questions { get; init; } = new();
}

/// <summary>
/// Response DTO for BookingQuestion.
/// </summary>
public record BookingQuestionResponse
{
    public Guid Id { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public List<string> Options { get; init; } = new();
    public int DisplayOrder { get; init; }
}
