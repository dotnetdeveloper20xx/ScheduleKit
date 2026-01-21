using Microsoft.Extensions.Logging;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// Mock calendar service that simulates Google Calendar / Outlook integration.
/// Stores calendar events locally and returns realistic responses.
/// </summary>
public class MockCalendarService : IExternalCalendarService
{
    private readonly ILogger<MockCalendarService> _logger;

    // In-memory store for mock calendar events (in real app, this would be persisted)
    private static readonly Dictionary<string, MockCalendarEvent> _events = new();
    private static readonly Dictionary<Guid, List<BusyTimeSlot>> _userBusyTimes = new();

    public MockCalendarService(ILogger<MockCalendarService> logger)
    {
        _logger = logger;
    }

    public string ProviderName => "Mock Calendar (Demo)";

    public Task<CalendarEventResult> CreateEventAsync(CalendarEventRequest request, CancellationToken cancellationToken = default)
    {
        var eventId = $"mock-cal-{Guid.NewGuid():N}";
        var calendarLink = $"https://calendar.mock.schedulekit.demo/event/{eventId}";

        var mockEvent = new MockCalendarEvent
        {
            ExternalEventId = eventId,
            BookingId = request.BookingId,
            HostUserId = request.HostUserId,
            Title = request.Title,
            Description = request.Description,
            StartTimeUtc = request.StartTimeUtc,
            EndTimeUtc = request.EndTimeUtc,
            Location = request.Location,
            GuestEmail = request.GuestEmail,
            MeetingUrl = request.MeetingUrl,
            CreatedAt = DateTime.UtcNow
        };

        _events[eventId] = mockEvent;

        _logger.LogInformation(
            "[MOCK] Calendar event created: {EventId} for booking {BookingId} - {Title} at {StartTime}",
            eventId, request.BookingId, request.Title, request.StartTimeUtc);

        return Task.FromResult(CalendarEventResult.Succeeded(eventId, calendarLink));
    }

    public Task<CalendarEventResult> UpdateEventAsync(string externalEventId, CalendarEventRequest request, CancellationToken cancellationToken = default)
    {
        if (!_events.TryGetValue(externalEventId, out var existingEvent))
        {
            _logger.LogWarning("[MOCK] Calendar event not found for update: {EventId}", externalEventId);
            return Task.FromResult(CalendarEventResult.Failed("Event not found"));
        }

        existingEvent.Title = request.Title;
        existingEvent.Description = request.Description;
        existingEvent.StartTimeUtc = request.StartTimeUtc;
        existingEvent.EndTimeUtc = request.EndTimeUtc;
        existingEvent.Location = request.Location;
        existingEvent.MeetingUrl = request.MeetingUrl;
        existingEvent.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "[MOCK] Calendar event updated: {EventId} - {Title} at {StartTime}",
            externalEventId, request.Title, request.StartTimeUtc);

        var calendarLink = $"https://calendar.mock.schedulekit.demo/event/{externalEventId}";
        return Task.FromResult(CalendarEventResult.Succeeded(externalEventId, calendarLink));
    }

    public Task<bool> DeleteEventAsync(string externalEventId, CancellationToken cancellationToken = default)
    {
        var removed = _events.Remove(externalEventId);

        _logger.LogInformation(
            "[MOCK] Calendar event {Status}: {EventId}",
            removed ? "deleted" : "not found for deletion", externalEventId);

        return Task.FromResult(removed);
    }

    public Task<IReadOnlyList<BusyTimeSlot>> GetBusyTimesAsync(Guid userId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default)
    {
        // Return any mock busy times for this user, plus events from our store
        var busyTimes = new List<BusyTimeSlot>();

        // Add events from our store that fall within the range
        var userEvents = _events.Values
            .Where(e => e.HostUserId == userId && e.StartTimeUtc < endUtc && e.EndTimeUtc > startUtc)
            .Select(e => new BusyTimeSlot
            {
                StartUtc = e.StartTimeUtc,
                EndUtc = e.EndTimeUtc,
                Title = e.Title
            });

        busyTimes.AddRange(userEvents);

        // Add any pre-configured busy times
        if (_userBusyTimes.TryGetValue(userId, out var configuredBusy))
        {
            busyTimes.AddRange(configuredBusy.Where(b => b.StartUtc < endUtc && b.EndUtc > startUtc));
        }

        _logger.LogDebug(
            "[MOCK] Retrieved {Count} busy times for user {UserId} between {Start} and {End}",
            busyTimes.Count, userId, startUtc, endUtc);

        return Task.FromResult<IReadOnlyList<BusyTimeSlot>>(busyTimes);
    }

    public Task<bool> IsConnectedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Mock is always "connected"
        _logger.LogDebug("[MOCK] Calendar connection check for user {UserId}: connected", userId);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Helper method to add mock busy times for testing (not part of interface).
    /// </summary>
    public static void AddMockBusyTime(Guid userId, DateTime startUtc, DateTime endUtc, string? title = null)
    {
        if (!_userBusyTimes.ContainsKey(userId))
        {
            _userBusyTimes[userId] = new List<BusyTimeSlot>();
        }

        _userBusyTimes[userId].Add(new BusyTimeSlot
        {
            StartUtc = startUtc,
            EndUtc = endUtc,
            Title = title ?? "Busy"
        });
    }

    private class MockCalendarEvent
    {
        public string ExternalEventId { get; set; } = string.Empty;
        public Guid BookingId { get; set; }
        public Guid HostUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public string? Location { get; set; }
        public string? GuestEmail { get; set; }
        public string? MeetingUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
