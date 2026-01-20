using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.Entities;

/// <summary>
/// Represents a host's weekly availability rule for a specific day.
/// Example: Monday 9:00 AM - 5:00 PM
/// </summary>
public class Availability : BaseEntity
{
    public Guid HostUserId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsEnabled { get; private set; }

    // For EF Core
    private Availability() { }

    private Availability(
        Guid hostUserId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isEnabled)
    {
        Id = Guid.NewGuid();
        HostUserId = hostUserId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        IsEnabled = isEnabled;
        SetCreatedAt();
    }

    /// <summary>
    /// Creates a new availability rule.
    /// </summary>
    public static Result<Availability> Create(
        Guid hostUserId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isEnabled = true)
    {
        if (hostUserId == Guid.Empty)
            return Result.Failure<Availability>("Host user ID is required.");

        if (endTime <= startTime)
            return Result.Failure<Availability>("End time must be after start time.");

        var duration = endTime - startTime;
        if (duration.TotalMinutes < 15)
            return Result.Failure<Availability>("Availability window must be at least 15 minutes.");

        return Result.Success(new Availability(hostUserId, dayOfWeek, startTime, endTime, isEnabled));
    }

    /// <summary>
    /// Creates default availability (9 AM - 5 PM, weekdays enabled).
    /// </summary>
    public static List<Availability> CreateDefaultWeek(Guid hostUserId)
    {
        var availabilities = new List<Availability>();
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(17, 0);

        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            var isWeekday = day is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
            availabilities.Add(new Availability(hostUserId, day, start, end, isWeekday));
        }

        return availabilities;
    }

    /// <summary>
    /// Updates the availability times.
    /// </summary>
    public Result Update(TimeOnly startTime, TimeOnly endTime, bool isEnabled)
    {
        if (endTime <= startTime)
            return Result.Failure("End time must be after start time.");

        var duration = endTime - startTime;
        if (duration.TotalMinutes < 15)
            return Result.Failure("Availability window must be at least 15 minutes.");

        StartTime = startTime;
        EndTime = endTime;
        IsEnabled = isEnabled;
        SetUpdatedAt();

        return Result.Success();
    }

    /// <summary>
    /// Enables this day's availability.
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Disables this day's availability.
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
        SetUpdatedAt();
    }

    /// <summary>
    /// Gets the duration of availability in minutes.
    /// </summary>
    public int GetDurationMinutes() => (int)(EndTime - StartTime).TotalMinutes;

    public override string ToString() =>
        $"{DayOfWeek}: {StartTime:HH:mm} - {EndTime:HH:mm} ({(IsEnabled ? "Enabled" : "Disabled")})";
}
