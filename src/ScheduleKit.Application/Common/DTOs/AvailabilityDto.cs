namespace ScheduleKit.Application.Common.DTOs;

/// <summary>
/// Response DTO for weekly availability.
/// </summary>
public record WeeklyAvailabilityResponse
{
    public Guid HostUserId { get; init; }
    public string Timezone { get; init; } = "UTC";
    public List<DayAvailabilityResponse> Days { get; init; } = new();
}

/// <summary>
/// Response DTO for a single day's availability.
/// </summary>
public record DayAvailabilityResponse
{
    public Guid Id { get; init; }
    public DayOfWeek DayOfWeek { get; init; }
    public string DayName => DayOfWeek.ToString();
    public string StartTime { get; init; } = string.Empty;
    public string EndTime { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
}

/// <summary>
/// Response DTO for availability override.
/// </summary>
public record AvailabilityOverrideResponse
{
    public Guid Id { get; init; }
    public Guid HostUserId { get; init; }
    public string Date { get; init; } = string.Empty;
    public string? StartTime { get; init; }
    public string? EndTime { get; init; }
    public bool IsBlocked { get; init; }
    public bool IsFullDayBlock { get; init; }
    public string? Reason { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

/// <summary>
/// Response DTO for available time slots.
/// </summary>
public record AvailableSlotsResponse
{
    public string Date { get; init; } = string.Empty;
    public string Timezone { get; init; } = string.Empty;
    public Guid EventTypeId { get; init; }
    public List<TimeSlotResponse> Slots { get; init; } = new();
}

/// <summary>
/// Response DTO for a single time slot.
/// </summary>
public record TimeSlotResponse
{
    public string StartTime { get; init; } = string.Empty;
    public string EndTime { get; init; } = string.Empty;
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public bool IsAvailable { get; init; } = true;
}

/// <summary>
/// Response DTO for available dates within booking window.
/// </summary>
public record AvailableDatesResponse
{
    public Guid EventTypeId { get; init; }
    public string FromDate { get; init; } = string.Empty;
    public string ToDate { get; init; } = string.Empty;
    public string Timezone { get; init; } = string.Empty;
    public List<DateAvailabilityResponse> Dates { get; init; } = new();
}

/// <summary>
/// Response DTO for a single date's availability status.
/// </summary>
public record DateAvailabilityResponse
{
    public string Date { get; init; } = string.Empty;
    public DayOfWeek DayOfWeek { get; init; }
    public bool HasAvailability { get; init; }
    public int AvailableSlotCount { get; init; }
}
