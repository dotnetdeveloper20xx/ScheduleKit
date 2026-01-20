using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.Entities;

/// <summary>
/// Represents a date-specific override to the host's regular availability.
/// Can block an entire day (vacation) or add/change availability for a specific date.
/// </summary>
public class AvailabilityOverride : BaseEntity
{
    public Guid HostUserId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly? StartTime { get; private set; }
    public TimeOnly? EndTime { get; private set; }
    public bool IsBlocked { get; private set; }
    public string? Reason { get; private set; }

    // For EF Core
    private AvailabilityOverride() { }

    private AvailabilityOverride(
        Guid hostUserId,
        DateOnly date,
        TimeOnly? startTime,
        TimeOnly? endTime,
        bool isBlocked,
        string? reason)
    {
        Id = Guid.NewGuid();
        HostUserId = hostUserId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        IsBlocked = isBlocked;
        Reason = reason;
        SetCreatedAt();
    }

    /// <summary>
    /// Creates an override that blocks the entire day.
    /// </summary>
    public static Result<AvailabilityOverride> CreateBlockedDay(
        Guid hostUserId,
        DateOnly date,
        string? reason = null)
    {
        if (hostUserId == Guid.Empty)
            return Result.Failure<AvailabilityOverride>("Host user ID is required.");

        if (date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            return Result.Failure<AvailabilityOverride>("Cannot create override for past dates.");

        if (reason?.Length > 200)
            return Result.Failure<AvailabilityOverride>("Reason is too long.");

        return Result.Success(new AvailabilityOverride(
            hostUserId,
            date,
            startTime: null,
            endTime: null,
            isBlocked: true,
            reason?.Trim()));
    }

    /// <summary>
    /// Creates an override that blocks a specific time range on a date.
    /// </summary>
    public static Result<AvailabilityOverride> CreateBlockedTimeRange(
        Guid hostUserId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string? reason = null)
    {
        if (hostUserId == Guid.Empty)
            return Result.Failure<AvailabilityOverride>("Host user ID is required.");

        if (date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            return Result.Failure<AvailabilityOverride>("Cannot create override for past dates.");

        if (endTime <= startTime)
            return Result.Failure<AvailabilityOverride>("End time must be after start time.");

        if (reason?.Length > 200)
            return Result.Failure<AvailabilityOverride>("Reason is too long.");

        return Result.Success(new AvailabilityOverride(
            hostUserId,
            date,
            startTime,
            endTime,
            isBlocked: true,
            reason?.Trim()));
    }

    /// <summary>
    /// Creates an override that adds extra availability on a date.
    /// Useful for working on a normally off day.
    /// </summary>
    public static Result<AvailabilityOverride> CreateExtraAvailability(
        Guid hostUserId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string? reason = null)
    {
        if (hostUserId == Guid.Empty)
            return Result.Failure<AvailabilityOverride>("Host user ID is required.");

        if (date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            return Result.Failure<AvailabilityOverride>("Cannot create override for past dates.");

        if (endTime <= startTime)
            return Result.Failure<AvailabilityOverride>("End time must be after start time.");

        var duration = endTime - startTime;
        if (duration.TotalMinutes < 15)
            return Result.Failure<AvailabilityOverride>("Extra availability must be at least 15 minutes.");

        if (reason?.Length > 200)
            return Result.Failure<AvailabilityOverride>("Reason is too long.");

        return Result.Success(new AvailabilityOverride(
            hostUserId,
            date,
            startTime,
            endTime,
            isBlocked: false,
            reason?.Trim()));
    }

    /// <summary>
    /// Checks if this override blocks the entire day.
    /// </summary>
    public bool IsFullDayBlock => IsBlocked && !StartTime.HasValue && !EndTime.HasValue;

    /// <summary>
    /// Checks if this override affects a specific time.
    /// </summary>
    public bool AffectsTime(TimeOnly time)
    {
        if (IsFullDayBlock)
            return true;

        if (StartTime.HasValue && EndTime.HasValue)
            return time >= StartTime.Value && time < EndTime.Value;

        return false;
    }

    public override string ToString()
    {
        if (IsFullDayBlock)
            return $"{Date:yyyy-MM-dd}: Blocked (Full Day) - {Reason ?? "No reason"}";

        var timeRange = $"{StartTime:HH:mm} - {EndTime:HH:mm}";
        var type = IsBlocked ? "Blocked" : "Available";
        return $"{Date:yyyy-MM-dd}: {type} {timeRange} - {Reason ?? "No reason"}";
    }
}
