using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents where a meeting takes place.
/// </summary>
public enum LocationType
{
    InPerson,
    Phone,
    Zoom,
    GoogleMeet,
    MicrosoftTeams,
    Custom
}

/// <summary>
/// Represents a meeting location with type and details.
/// </summary>
public sealed record MeetingLocation
{
    public LocationType Type { get; }
    public string? Details { get; }
    public string? DisplayName { get; }

    private MeetingLocation(LocationType type, string? details, string? displayName)
    {
        Type = type;
        Details = details;
        DisplayName = displayName;
    }

    public static Result<MeetingLocation> CreateInPerson(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return Result.Failure<MeetingLocation>("Address is required for in-person meetings.");

        if (address.Length > 500)
            return Result.Failure<MeetingLocation>("Address is too long.");

        return Result.Success(new MeetingLocation(LocationType.InPerson, address, "In Person"));
    }

    public static Result<MeetingLocation> CreatePhone(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure<MeetingLocation>("Phone number is required.");

        return Result.Success(new MeetingLocation(LocationType.Phone, phoneNumber, "Phone Call"));
    }

    public static MeetingLocation CreateZoom(string? meetingLink = null)
    {
        return new MeetingLocation(LocationType.Zoom, meetingLink, "Zoom");
    }

    public static MeetingLocation CreateGoogleMeet(string? meetingLink = null)
    {
        return new MeetingLocation(LocationType.GoogleMeet, meetingLink, "Google Meet");
    }

    public static MeetingLocation CreateMicrosoftTeams(string? meetingLink = null)
    {
        return new MeetingLocation(LocationType.MicrosoftTeams, meetingLink, "Microsoft Teams");
    }

    public static Result<MeetingLocation> CreateCustom(string displayName, string? details = null)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Failure<MeetingLocation>("Display name is required for custom location.");

        return Result.Success(new MeetingLocation(LocationType.Custom, details, displayName));
    }

    /// <summary>
    /// Creates from stored values. Use only when loading from database.
    /// </summary>
    public static MeetingLocation FromStorage(LocationType type, string? details, string? displayName)
    {
        return new MeetingLocation(type, details, displayName);
    }

    public override string ToString() => DisplayName ?? Type.ToString();
}
