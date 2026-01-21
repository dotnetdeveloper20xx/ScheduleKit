using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents the minimum advance notice required for bookings.
/// Prevents last-minute bookings by requiring guests to book a certain time in advance.
/// </summary>
public sealed record MinimumNotice
{
    public const int MaxMinutes = 10080; // 7 days in minutes

    public int Minutes { get; }

    private MinimumNotice(int minutes)
    {
        Minutes = minutes;
    }

    public static Result<MinimumNotice> Create(int minutes)
    {
        if (minutes < 0)
            return Result.Failure<MinimumNotice>("Minimum notice cannot be negative.");

        if (minutes > MaxMinutes)
            return Result.Failure<MinimumNotice>($"Minimum notice cannot exceed {MaxMinutes} minutes (7 days).");

        return Result.Success(new MinimumNotice(minutes));
    }

    /// <summary>
    /// Creates a MinimumNotice without validation. Use only when loading from database.
    /// </summary>
    public static MinimumNotice FromMinutes(int minutes) => new(minutes);

    // Common presets
    public static MinimumNotice None => new(0);
    public static MinimumNotice FifteenMinutes => new(15);
    public static MinimumNotice ThirtyMinutes => new(30);
    public static MinimumNotice OneHour => new(60);
    public static MinimumNotice TwoHours => new(120);
    public static MinimumNotice FourHours => new(240);
    public static MinimumNotice TwelveHours => new(720);
    public static MinimumNotice TwentyFourHours => new(1440);
    public static MinimumNotice FortyEightHours => new(2880);

    public TimeSpan ToTimeSpan() => TimeSpan.FromMinutes(Minutes);

    public override string ToString() => Minutes switch
    {
        0 => "No minimum notice",
        < 60 => $"{Minutes} minutes",
        60 => "1 hour",
        < 1440 => $"{Minutes / 60} hours",
        1440 => "1 day",
        _ => $"{Minutes / 1440} days"
    };
}
