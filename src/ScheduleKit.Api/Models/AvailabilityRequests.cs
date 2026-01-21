namespace ScheduleKit.Api.Models;

/// <summary>
/// Request model for updating weekly availability.
/// </summary>
public record UpdateWeeklyAvailabilityRequest
{
    public List<DayAvailabilityRequest> Days { get; init; } = new();
}

/// <summary>
/// Request model for a single day's availability.
/// </summary>
public record DayAvailabilityRequest
{
    public DayOfWeek DayOfWeek { get; init; }
    public string StartTime { get; init; } = string.Empty;
    public string EndTime { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
}

/// <summary>
/// Request model for creating an availability override.
/// </summary>
public record CreateAvailabilityOverrideRequest
{
    /// <summary>
    /// Date in yyyy-MM-dd format.
    /// </summary>
    public string Date { get; init; } = string.Empty;

    /// <summary>
    /// Start time in HH:mm format (optional for full day block).
    /// </summary>
    public string? StartTime { get; init; }

    /// <summary>
    /// End time in HH:mm format (optional for full day block).
    /// </summary>
    public string? EndTime { get; init; }

    /// <summary>
    /// True to block the time, false to add extra availability.
    /// </summary>
    public bool IsBlocked { get; init; } = true;

    /// <summary>
    /// Optional reason for the override.
    /// </summary>
    public string? Reason { get; init; }
}
