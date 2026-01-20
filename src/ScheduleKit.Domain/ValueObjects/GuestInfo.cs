using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents guest information for a booking.
/// </summary>
public sealed record GuestInfo
{
    public string Name { get; }
    public Email Email { get; }
    public string? Phone { get; }
    public string Timezone { get; }

    private GuestInfo(string name, Email email, string? phone, string timezone)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Timezone = timezone;
    }

    public static Result<GuestInfo> Create(string name, string email, string? phone, string timezone)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<GuestInfo>("Guest name is required.");

        if (name.Length > 200)
            return Result.Failure<GuestInfo>("Guest name is too long.");

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<GuestInfo>(emailResult.Error);

        if (!string.IsNullOrEmpty(phone) && phone.Length > 50)
            return Result.Failure<GuestInfo>("Phone number is too long.");

        if (string.IsNullOrWhiteSpace(timezone))
            return Result.Failure<GuestInfo>("Timezone is required.");

        // Validate timezone
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            return Result.Failure<GuestInfo>($"Invalid timezone: {timezone}");
        }

        return Result.Success(new GuestInfo(name.Trim(), emailResult.Value, phone?.Trim(), timezone));
    }

    /// <summary>
    /// Creates from stored values. Use only when loading from database.
    /// </summary>
    public static GuestInfo FromStorage(string name, string email, string? phone, string timezone)
    {
        return new GuestInfo(name, Email.FromString(email), phone, timezone);
    }

    public override string ToString() => $"{Name} ({Email})";
}
