using Microsoft.Extensions.Logging;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// Mock video conferencing service that simulates Zoom, Google Meet, and Microsoft Teams.
/// Generates realistic-looking meeting URLs and information.
/// </summary>
public class MockVideoConferenceService : IVideoConferenceService
{
    private readonly ILogger<MockVideoConferenceService> _logger;

    // In-memory store for mock meetings
    private static readonly Dictionary<string, MockMeeting> _meetings = new();

    public MockVideoConferenceService(ILogger<MockVideoConferenceService> logger)
    {
        _logger = logger;
    }

    public string ProviderName => "Mock Video Conference (Demo)";

    public IReadOnlyList<string> SupportedLocationTypes => new[]
    {
        "Zoom",
        "GoogleMeet",
        "MicrosoftTeams"
    };

    public Task<MeetingResult> CreateMeetingAsync(MeetingRequest request, CancellationToken cancellationToken = default)
    {
        var meetingId = GenerateMeetingId(request.LocationType);
        var (joinUrl, hostUrl, password) = GenerateMeetingUrls(request.LocationType, meetingId);

        var mockMeeting = new MockMeeting
        {
            ExternalMeetingId = meetingId,
            BookingId = request.BookingId,
            HostUserId = request.HostUserId,
            Title = request.Title,
            StartTimeUtc = request.StartTimeUtc,
            EndTimeUtc = request.EndTimeUtc,
            LocationType = request.LocationType,
            JoinUrl = joinUrl,
            HostUrl = hostUrl,
            Password = password,
            CreatedAt = DateTime.UtcNow
        };

        _meetings[meetingId] = mockMeeting;

        _logger.LogInformation(
            "[MOCK] {Provider} meeting created: {MeetingId} for booking {BookingId} - Join URL: {JoinUrl}",
            request.LocationType, meetingId, request.BookingId, joinUrl);

        return Task.FromResult(MeetingResult.Succeeded(meetingId, joinUrl, hostUrl, password));
    }

    public Task<MeetingResult> UpdateMeetingAsync(string externalMeetingId, MeetingRequest request, CancellationToken cancellationToken = default)
    {
        if (!_meetings.TryGetValue(externalMeetingId, out var existingMeeting))
        {
            _logger.LogWarning("[MOCK] Meeting not found for update: {MeetingId}", externalMeetingId);
            return Task.FromResult(MeetingResult.Failed("Meeting not found"));
        }

        existingMeeting.Title = request.Title;
        existingMeeting.StartTimeUtc = request.StartTimeUtc;
        existingMeeting.EndTimeUtc = request.EndTimeUtc;
        existingMeeting.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "[MOCK] Meeting updated: {MeetingId} - {Title}",
            externalMeetingId, request.Title);

        return Task.FromResult(MeetingResult.Succeeded(
            existingMeeting.ExternalMeetingId,
            existingMeeting.JoinUrl,
            existingMeeting.HostUrl,
            existingMeeting.Password));
    }

    public Task<bool> DeleteMeetingAsync(string externalMeetingId, CancellationToken cancellationToken = default)
    {
        var removed = _meetings.Remove(externalMeetingId);

        _logger.LogInformation(
            "[MOCK] Meeting {Status}: {MeetingId}",
            removed ? "deleted" : "not found for deletion", externalMeetingId);

        return Task.FromResult(removed);
    }

    public Task<bool> IsConnectedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Mock is always "connected"
        _logger.LogDebug("[MOCK] Video conference connection check for user {UserId}: connected", userId);
        return Task.FromResult(true);
    }

    private static string GenerateMeetingId(string locationType)
    {
        return locationType switch
        {
            "Zoom" => $"{Random.Shared.NextInt64(100_000_000, 999_999_999)}", // Zoom-style meeting ID
            "GoogleMeet" => $"{GenerateRandomString(3)}-{GenerateRandomString(4)}-{GenerateRandomString(3)}", // xxx-xxxx-xxx
            "MicrosoftTeams" => Guid.NewGuid().ToString("N")[..12], // Teams-style ID
            _ => Guid.NewGuid().ToString("N")[..10]
        };
    }

    private static (string joinUrl, string? hostUrl, string? password) GenerateMeetingUrls(string locationType, string meetingId)
    {
        return locationType switch
        {
            "Zoom" => (
                $"https://zoom.mock.schedulekit.demo/j/{meetingId}",
                $"https://zoom.mock.schedulekit.demo/s/{meetingId}",
                GenerateRandomString(6)
            ),
            "GoogleMeet" => (
                $"https://meet.mock.schedulekit.demo/{meetingId}",
                null,
                null
            ),
            "MicrosoftTeams" => (
                $"https://teams.mock.schedulekit.demo/l/meetup-join/{meetingId}",
                null,
                null
            ),
            _ => (
                $"https://meeting.mock.schedulekit.demo/{meetingId}",
                null,
                null
            )
        };
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[Random.Shared.Next(chars.Length)])
            .ToArray());
    }

    private class MockMeeting
    {
        public string ExternalMeetingId { get; set; } = string.Empty;
        public Guid BookingId { get; set; }
        public Guid HostUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public string LocationType { get; set; } = string.Empty;
        public string JoinUrl { get; set; } = string.Empty;
        public string? HostUrl { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
