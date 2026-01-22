using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Events;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Infrastructure.Data;

/// <summary>
/// Seeds the database with demo data for testing and demonstration.
/// </summary>
public class DatabaseSeeder
{
    private readonly ScheduleKitDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ScheduleKitDbContext context, IAuthService authService, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Check if already seeded
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Database already seeded");
                return;
            }

            _logger.LogInformation("Seeding database...");

            // Create demo host user
            var passwordHash = _authService.HashPassword("Demo123!");
            var demoHostResult = User.Create(
                "demo@schedulekit.com",
                passwordHash,
                "Afzal Ahmed",
                "afzal-ahmed"
            );

            if (demoHostResult.IsFailure)
            {
                _logger.LogError("Failed to create demo host: {Error}", demoHostResult.Error);
                return;
            }

            var demoHost = demoHostResult.Value;
            demoHost.UpdateTimezone("America/New_York");
            _context.Users.Add(demoHost);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created demo host: {Email}", demoHost.Email);

            // Create event types
            var eventTypes = await CreateEventTypesAsync(demoHost.Id);
            _logger.LogInformation("Created {Count} event types", eventTypes.Count);

            // Create default availability (Mon-Fri 9-5)
            var availabilities = Availability.CreateDefaultWeek(demoHost.Id);
            _context.Availabilities.AddRange(availabilities);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created default availability");

            // Create availability overrides
            await CreateAvailabilityOverridesAsync(demoHost.Id);

            // Create sample bookings
            await CreateSampleBookingsAsync(eventTypes);

            _logger.LogInformation("Database seeded successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            throw;
        }
    }

    private async Task<List<EventType>> CreateEventTypesAsync(Guid hostUserId)
    {
        var eventTypes = new List<EventType>();

        // 15 Minute Quick Chat
        var quickChatResult = EventType.Create(
            hostUserId,
            "15 Minute Quick Chat",
            15,
            MeetingLocation.CreateGoogleMeet(),
            "A quick 15-minute call to discuss a simple question or topic.",
            0, 5, 60, 30, null, "#10B981"
        );
        if (quickChatResult.IsSuccess) eventTypes.Add(quickChatResult.Value);

        // 30 Minute Consultation
        var consultationResult = EventType.Create(
            hostUserId,
            "30 Minute Consultation",
            30,
            MeetingLocation.CreateGoogleMeet(),
            "A focused 30-minute consultation to discuss your needs and how I can help.",
            5, 5, 24 * 60, 60, 5, "#3B82F6"
        );
        if (consultationResult.IsSuccess) eventTypes.Add(consultationResult.Value);

        // 60 Minute Deep Dive
        var deepDiveResult = EventType.Create(
            hostUserId,
            "60 Minute Deep Dive",
            60,
            MeetingLocation.CreateZoom("https://zoom.us/j/demo"),
            "An in-depth 60-minute session to thoroughly explore a topic or project.",
            10, 10, 48 * 60, 60, 3, "#8B5CF6"
        );
        if (deepDiveResult.IsSuccess) eventTypes.Add(deepDiveResult.Value);

        // Technical Interview
        var interviewResult = EventType.Create(
            hostUserId,
            "Technical Interview",
            45,
            MeetingLocation.CreateGoogleMeet(),
            "A 45-minute technical interview session for candidates.",
            15, 15, 72 * 60, 30, 2, "#F59E0B"
        );
        if (interviewResult.IsSuccess) eventTypes.Add(interviewResult.Value);

        // Inactive Coffee Chat
        var phoneResult = MeetingLocation.CreatePhone("+1-555-0123");
        if (phoneResult.IsSuccess)
        {
            var coffeeChatResult = EventType.Create(
                hostUserId,
                "Coffee Chat (Inactive)",
                20,
                phoneResult.Value,
                "An informal coffee chat - currently not accepting bookings.",
                0, 0, 60, 14, null, "#6B7280"
            );
            if (coffeeChatResult.IsSuccess)
            {
                coffeeChatResult.Value.Deactivate();
                eventTypes.Add(coffeeChatResult.Value);
            }
        }

        _context.EventTypes.AddRange(eventTypes);
        await _context.SaveChangesAsync();
        return eventTypes;
    }

    private async Task CreateAvailabilityOverridesAsync(Guid hostUserId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Block next Friday (vacation/team offsite)
        var nextFriday = today.AddDays(GetDaysUntilDayOfWeek(today, DayOfWeek.Friday) + 7);
        var blockResult = AvailabilityOverride.CreateBlockedDay(
            hostUserId, nextFriday, "Team offsite - unavailable"
        );
        if (blockResult.IsSuccess)
        {
            _context.AvailabilityOverrides.Add(blockResult.Value);
        }

        // Extended hours next Saturday (special availability)
        var nextSaturday = today.AddDays(GetDaysUntilDayOfWeek(today, DayOfWeek.Saturday) + 7);
        var extendedResult = AvailabilityOverride.CreateExtraAvailability(
            hostUserId, nextSaturday,
            new TimeOnly(8, 0), new TimeOnly(18, 0),
            "Special availability for project deadline"
        );
        if (extendedResult.IsSuccess)
        {
            _context.AvailabilityOverrides.Add(extendedResult.Value);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Created availability overrides");
    }

    private async Task CreateSampleBookingsAsync(List<EventType> eventTypes)
    {
        var consultationEvent = eventTypes.FirstOrDefault(e => e.Name.Contains("Consultation"));
        var quickChatEvent = eventTypes.FirstOrDefault(e => e.Name.Contains("Quick Chat"));
        var deepDiveEvent = eventTypes.FirstOrDefault(e => e.Name.Contains("Deep Dive"));

        if (consultationEvent == null || quickChatEvent == null || deepDiveEvent == null)
        {
            _logger.LogWarning("Could not find all event types for bookings");
            return;
        }

        var bookings = new List<Booking>();
        var now = DateTime.UtcNow;

        // Upcoming booking - Tomorrow
        var tomorrowSlot = TimeSlot.Create(
            DateTime.SpecifyKind(now.AddDays(1).Date.AddHours(15), DateTimeKind.Utc),
            DateTime.SpecifyKind(now.AddDays(1).Date.AddHours(15).AddMinutes(30), DateTimeKind.Utc)
        );
        var tomorrowGuest = GuestInfo.Create("Alex Rivera", "alex@enterprise.com", "+1-555-1003", "America/Chicago");
        if (tomorrowSlot.IsSuccess && tomorrowGuest.IsSuccess)
        {
            var bookingResult = Booking.Create(
                consultationEvent, tomorrowGuest.Value, tomorrowSlot.Value,
                "Want to discuss integration options for our platform",
                "https://meet.google.com/tomorrow-meeting"
            );
            if (bookingResult.IsSuccess) bookings.Add(bookingResult.Value);
        }

        // Day after tomorrow - Deep dive
        var dayAfterSlot = TimeSlot.Create(
            DateTime.SpecifyKind(now.AddDays(2).Date.AddHours(19), DateTimeKind.Utc),
            DateTime.SpecifyKind(now.AddDays(2).Date.AddHours(20), DateTimeKind.Utc)
        );
        var dayAfterGuest = GuestInfo.Create("Emily Watson", "emily@consultant.co", "+1-555-1004", "America/New_York");
        if (dayAfterSlot.IsSuccess && dayAfterGuest.IsSuccess)
        {
            var bookingResult = Booking.Create(
                deepDiveEvent, dayAfterGuest.Value, dayAfterSlot.Value,
                "Architecture review for the new microservices migration",
                "https://zoom.us/j/123456789"
            );
            if (bookingResult.IsSuccess) bookings.Add(bookingResult.Value);
        }

        // Next week booking
        var nextWeekSlot = TimeSlot.Create(
            DateTime.SpecifyKind(now.AddDays(7).Date.AddHours(14), DateTimeKind.Utc),
            DateTime.SpecifyKind(now.AddDays(7).Date.AddHours(14).AddMinutes(15), DateTimeKind.Utc)
        );
        var nextWeekGuest = GuestInfo.Create("Robert Kim", "robert@fintech.io", null, "Asia/Tokyo");
        if (nextWeekSlot.IsSuccess && nextWeekGuest.IsSuccess)
        {
            var bookingResult = Booking.Create(
                quickChatEvent, nextWeekGuest.Value, nextWeekSlot.Value,
                "Quick sync on API requirements",
                "https://meet.google.com/nxt-week-01"
            );
            if (bookingResult.IsSuccess) bookings.Add(bookingResult.Value);
        }

        // Another next week booking
        var nextWeek2Slot = TimeSlot.Create(
            DateTime.SpecifyKind(now.AddDays(8).Date.AddHours(16), DateTimeKind.Utc),
            DateTime.SpecifyKind(now.AddDays(8).Date.AddHours(16).AddMinutes(30), DateTimeKind.Utc)
        );
        var nextWeek2Guest = GuestInfo.Create("Lisa Park", "lisa@agency.com", "+1-555-1005", "America/New_York");
        if (nextWeek2Slot.IsSuccess && nextWeek2Guest.IsSuccess)
        {
            var bookingResult = Booking.Create(
                consultationEvent, nextWeek2Guest.Value, nextWeek2Slot.Value,
                "Marketing campaign review",
                null
            );
            if (bookingResult.IsSuccess) bookings.Add(bookingResult.Value);
        }

        // Cancelled booking (future date so we can cancel it)
        var cancelledSlot = TimeSlot.Create(
            DateTime.SpecifyKind(now.AddDays(5).Date.AddHours(15), DateTimeKind.Utc),
            DateTime.SpecifyKind(now.AddDays(5).Date.AddHours(15).AddMinutes(30), DateTimeKind.Utc)
        );
        var cancelledGuest = GuestInfo.Create("Jane Doe", "jane@example.org", "+1-555-1002", "Europe/London");
        if (cancelledSlot.IsSuccess && cancelledGuest.IsSuccess)
        {
            var bookingResult = Booking.Create(
                consultationEvent, cancelledGuest.Value, cancelledSlot.Value,
                "Need to reschedule due to conflict",
                "https://meet.google.com/xyz-uvwx-rst"
            );
            if (bookingResult.IsSuccess)
            {
                bookingResult.Value.Cancel("Schedule conflict - will reschedule", CancelledBy.Guest);
                bookings.Add(bookingResult.Value);
            }
        }

        _context.Bookings.AddRange(bookings);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} sample bookings", bookings.Count);
    }

    private static int GetDaysUntilDayOfWeek(DateOnly from, DayOfWeek target)
    {
        int daysUntil = ((int)target - (int)from.DayOfWeek + 7) % 7;
        return daysUntil == 0 ? 7 : daysUntil;
    }
}
