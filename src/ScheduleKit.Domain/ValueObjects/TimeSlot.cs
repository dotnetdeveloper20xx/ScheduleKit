using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents a time slot with start and end times.
/// All times are stored in UTC internally.
/// </summary>
public sealed record TimeSlot
{
    public DateTime StartTimeUtc { get; }
    public DateTime EndTimeUtc { get; }

    private TimeSlot(DateTime startTimeUtc, DateTime endTimeUtc)
    {
        StartTimeUtc = startTimeUtc;
        EndTimeUtc = endTimeUtc;
    }

    public static Result<TimeSlot> Create(DateTime startTimeUtc, DateTime endTimeUtc)
    {
        if (startTimeUtc.Kind != DateTimeKind.Utc)
            return Result.Failure<TimeSlot>("Start time must be in UTC.");

        if (endTimeUtc.Kind != DateTimeKind.Utc)
            return Result.Failure<TimeSlot>("End time must be in UTC.");

        if (endTimeUtc <= startTimeUtc)
            return Result.Failure<TimeSlot>("End time must be after start time.");

        return Result.Success(new TimeSlot(startTimeUtc, endTimeUtc));
    }

    public static Result<TimeSlot> Create(DateTime startTimeUtc, Duration duration)
    {
        if (startTimeUtc.Kind != DateTimeKind.Utc)
            return Result.Failure<TimeSlot>("Start time must be in UTC.");

        var endTimeUtc = startTimeUtc.Add(duration.ToTimeSpan());
        return Result.Success(new TimeSlot(startTimeUtc, endTimeUtc));
    }

    /// <summary>
    /// Creates from stored values. Use only when loading from database.
    /// </summary>
    public static TimeSlot FromStorage(DateTime startTimeUtc, DateTime endTimeUtc)
    {
        return new TimeSlot(
            DateTime.SpecifyKind(startTimeUtc, DateTimeKind.Utc),
            DateTime.SpecifyKind(endTimeUtc, DateTimeKind.Utc));
    }

    public TimeSpan Duration => EndTimeUtc - StartTimeUtc;

    public bool OverlapsWith(TimeSlot other)
    {
        return StartTimeUtc < other.EndTimeUtc && EndTimeUtc > other.StartTimeUtc;
    }

    public bool Contains(DateTime utcTime)
    {
        return utcTime >= StartTimeUtc && utcTime < EndTimeUtc;
    }

    public bool IsBefore(DateTime utcTime)
    {
        return EndTimeUtc <= utcTime;
    }

    public bool IsAfter(DateTime utcTime)
    {
        return StartTimeUtc >= utcTime;
    }

    /// <summary>
    /// Creates a new TimeSlot with buffer times applied.
    /// </summary>
    public TimeSlot WithBuffers(BufferTime before, BufferTime after)
    {
        return new TimeSlot(
            StartTimeUtc.Subtract(before.ToTimeSpan()),
            EndTimeUtc.Add(after.ToTimeSpan()));
    }

    public override string ToString()
    {
        return $"{StartTimeUtc:yyyy-MM-dd HH:mm} - {EndTimeUtc:HH:mm} UTC";
    }
}
