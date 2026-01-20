using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents a meeting duration.
/// Immutable value object with validation.
/// </summary>
public sealed record Duration
{
    public const int MinMinutes = 15;
    public const int MaxMinutes = 480; // 8 hours

    public int Minutes { get; }

    private Duration(int minutes)
    {
        Minutes = minutes;
    }

    public static Result<Duration> Create(int minutes)
    {
        if (minutes < MinMinutes)
            return Result.Failure<Duration>($"Duration must be at least {MinMinutes} minutes.");

        if (minutes > MaxMinutes)
            return Result.Failure<Duration>($"Duration cannot exceed {MaxMinutes} minutes (8 hours).");

        if (minutes % 5 != 0)
            return Result.Failure<Duration>("Duration must be in 5-minute increments.");

        return Result.Success(new Duration(minutes));
    }

    /// <summary>
    /// Creates a Duration without validation. Use only when loading from database.
    /// </summary>
    public static Duration FromMinutes(int minutes) => new(minutes);

    // Common durations
    public static Duration FifteenMinutes => new(15);
    public static Duration ThirtyMinutes => new(30);
    public static Duration FortyFiveMinutes => new(45);
    public static Duration OneHour => new(60);
    public static Duration NinetyMinutes => new(90);
    public static Duration TwoHours => new(120);

    public TimeSpan ToTimeSpan() => TimeSpan.FromMinutes(Minutes);

    public override string ToString() => Minutes switch
    {
        < 60 => $"{Minutes} min",
        60 => "1 hour",
        _ when Minutes % 60 == 0 => $"{Minutes / 60} hours",
        _ => $"{Minutes / 60}h {Minutes % 60}m"
    };
}
