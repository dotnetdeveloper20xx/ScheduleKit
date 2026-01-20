namespace ScheduleKit.Api.Models;

/// <summary>
/// Request model for creating a new event type.
/// </summary>
public record CreateEventTypeRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DurationMinutes { get; init; }
    public int BufferBeforeMinutes { get; init; }
    public int BufferAfterMinutes { get; init; }
    public string LocationType { get; init; } = string.Empty;
    public string? LocationDetails { get; init; }
    public string? Color { get; init; }
}

/// <summary>
/// Request model for updating an existing event type.
/// </summary>
public record UpdateEventTypeRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DurationMinutes { get; init; }
    public int BufferBeforeMinutes { get; init; }
    public int BufferAfterMinutes { get; init; }
    public string LocationType { get; init; } = string.Empty;
    public string? LocationDetails { get; init; }
    public string? Color { get; init; }
}
