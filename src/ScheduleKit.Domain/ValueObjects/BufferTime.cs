using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents buffer time before or after a meeting.
/// Prevents back-to-back bookings.
/// </summary>
public sealed record BufferTime
{
    public const int MaxMinutes = 120; // 2 hours

    public int Minutes { get; }

    private BufferTime(int minutes)
    {
        Minutes = minutes;
    }

    public static Result<BufferTime> Create(int minutes)
    {
        if (minutes < 0)
            return Result.Failure<BufferTime>("Buffer time cannot be negative.");

        if (minutes > MaxMinutes)
            return Result.Failure<BufferTime>($"Buffer time cannot exceed {MaxMinutes} minutes.");

        if (minutes % 5 != 0)
            return Result.Failure<BufferTime>("Buffer time must be in 5-minute increments.");

        return Result.Success(new BufferTime(minutes));
    }

    /// <summary>
    /// Creates a BufferTime without validation. Use only when loading from database.
    /// </summary>
    public static BufferTime FromMinutes(int minutes) => new(minutes);

    public static BufferTime None => new(0);
    public static BufferTime FiveMinutes => new(5);
    public static BufferTime TenMinutes => new(10);
    public static BufferTime FifteenMinutes => new(15);
    public static BufferTime ThirtyMinutes => new(30);

    public TimeSpan ToTimeSpan() => TimeSpan.FromMinutes(Minutes);

    public override string ToString() => Minutes == 0 ? "No buffer" : $"{Minutes} min buffer";
}
