using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents how far in advance guests can book appointments.
/// Controls the booking horizon (e.g., 60 days ahead).
/// </summary>
public sealed record BookingWindow
{
    public const int MinDays = 1;
    public const int MaxDays = 365;

    public int Days { get; }

    private BookingWindow(int days)
    {
        Days = days;
    }

    public static Result<BookingWindow> Create(int days)
    {
        if (days < MinDays)
            return Result.Failure<BookingWindow>($"Booking window must be at least {MinDays} day.");

        if (days > MaxDays)
            return Result.Failure<BookingWindow>($"Booking window cannot exceed {MaxDays} days.");

        return Result.Success(new BookingWindow(days));
    }

    /// <summary>
    /// Creates a BookingWindow without validation. Use only when loading from database.
    /// </summary>
    public static BookingWindow FromDays(int days) => new(days);

    // Common presets
    public static BookingWindow OneWeek => new(7);
    public static BookingWindow TwoWeeks => new(14);
    public static BookingWindow ThirtyDays => new(30);
    public static BookingWindow SixtyDays => new(60);
    public static BookingWindow NinetyDays => new(90);
    public static BookingWindow SixMonths => new(180);
    public static BookingWindow OneYear => new(365);

    public DateOnly GetMaxBookableDate(DateOnly fromDate) => fromDate.AddDays(Days);

    public override string ToString() => Days switch
    {
        7 => "1 week",
        14 => "2 weeks",
        30 => "30 days",
        60 => "60 days",
        90 => "90 days",
        180 => "6 months",
        365 => "1 year",
        _ => $"{Days} days"
    };
}
