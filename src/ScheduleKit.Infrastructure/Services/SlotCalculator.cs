using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Services;

namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// Implementation of slot calculation logic.
/// Handles timezone conversions, availability checks, and conflict detection.
/// </summary>
public class SlotCalculator : ISlotCalculator
{
    /// <inheritdoc />
    public List<CalculatedSlot> CalculateSlotsForDate(
        EventType eventType,
        List<Availability> availabilities,
        List<AvailabilityOverride> overrides,
        List<Booking> existingBookings,
        DateOnly date,
        string hostTimezone)
    {
        var slots = new List<CalculatedSlot>();

        // Get the timezone info
        var timeZoneInfo = GetTimeZoneInfo(hostTimezone);

        // Check if the entire day is blocked by an override
        var dayOverrides = overrides.Where(o => o.Date == date).ToList();
        if (dayOverrides.Any(o => o.IsFullDayBlock))
        {
            return slots; // No slots available - full day blocked
        }

        // Check max bookings per day limit
        if (eventType.MaxBookingsPerDay.HasValue)
        {
            var confirmedBookingsOnDate = existingBookings
                .Count(b => DateOnly.FromDateTime(b.StartTimeUtc) == date &&
                           b.Status != BookingStatus.Cancelled);
            if (confirmedBookingsOnDate >= eventType.MaxBookingsPerDay.Value)
            {
                return slots; // No slots available - max bookings reached for this day
            }
        }

        // Get base availability for this day of week
        var dayAvailability = availabilities.FirstOrDefault(a => a.DayOfWeek == date.DayOfWeek);

        // Check for extra availability override (adds time on normally off days)
        var extraAvailability = dayOverrides.FirstOrDefault(o => !o.IsBlocked);

        TimeOnly baseStartTime, baseEndTime;

        if (extraAvailability != null && extraAvailability.StartTime.HasValue && extraAvailability.EndTime.HasValue)
        {
            // Use extra availability
            baseStartTime = extraAvailability.StartTime.Value;
            baseEndTime = extraAvailability.EndTime.Value;
        }
        else if (dayAvailability != null && dayAvailability.IsEnabled)
        {
            // Use regular availability
            baseStartTime = dayAvailability.StartTime;
            baseEndTime = dayAvailability.EndTime;
        }
        else
        {
            return slots; // No availability for this day
        }

        // Get blocked time ranges for this day
        var blockedRanges = dayOverrides
            .Where(o => o.IsBlocked && o.StartTime.HasValue && o.EndTime.HasValue)
            .Select(o => (Start: o.StartTime!.Value, End: o.EndTime!.Value))
            .ToList();

        // Calculate event duration including buffers
        var totalDuration = eventType.Duration.Minutes +
                           eventType.BufferBefore.Minutes +
                           eventType.BufferAfter.Minutes;

        var slotInterval = 15; // Slots start every 15 minutes
        var currentTime = baseStartTime;

        // Get minimum notice cutoff (in host timezone)
        var nowUtc = DateTime.UtcNow;
        var nowInHostTz = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZoneInfo);
        var minimumNoticeMinutes = eventType.MinimumNotice.Minutes;
        var minimumNoticeEnd = nowInHostTz.AddMinutes(minimumNoticeMinutes);

        while (currentTime.AddMinutes(eventType.Duration.Minutes) <= baseEndTime)
        {
            var slotStart = currentTime;
            var slotEnd = currentTime.AddMinutes(eventType.Duration.Minutes);

            // Convert to DateTime for UTC conversion
            var slotStartDateTime = date.ToDateTime(slotStart);
            var slotEndDateTime = date.ToDateTime(slotEnd);

            // Convert to UTC
            var slotStartUtc = TimeZoneInfo.ConvertTimeToUtc(slotStartDateTime, timeZoneInfo);
            var slotEndUtc = TimeZoneInfo.ConvertTimeToUtc(slotEndDateTime, timeZoneInfo);

            // Check if slot is available
            var isAvailable = IsSlotTimeAvailable(
                slotStart,
                slotEnd,
                eventType.BufferBefore.Minutes,
                eventType.BufferAfter.Minutes,
                blockedRanges,
                existingBookings,
                date,
                timeZoneInfo);

            // Check minimum notice
            if (slotStartDateTime <= minimumNoticeEnd)
            {
                isAvailable = false;
            }

            // Check if slot is in the past
            if (slotStartUtc <= nowUtc)
            {
                isAvailable = false;
            }

            slots.Add(new CalculatedSlot
            {
                StartTime = slotStart,
                EndTime = slotEnd,
                StartTimeUtc = slotStartUtc,
                EndTimeUtc = slotEndUtc,
                IsAvailable = isAvailable
            });

            currentTime = currentTime.AddMinutes(slotInterval);
        }

        return slots;
    }

    /// <inheritdoc />
    public bool IsSlotAvailable(
        EventType eventType,
        List<Availability> availabilities,
        List<AvailabilityOverride> overrides,
        List<Booking> existingBookings,
        DateTime proposedStartUtc,
        string hostTimezone)
    {
        var timeZoneInfo = GetTimeZoneInfo(hostTimezone);
        var proposedStartLocal = TimeZoneInfo.ConvertTimeFromUtc(proposedStartUtc, timeZoneInfo);
        var date = DateOnly.FromDateTime(proposedStartLocal);

        var slots = CalculateSlotsForDate(
            eventType,
            availabilities,
            overrides,
            existingBookings,
            date,
            hostTimezone);

        return slots.Any(s =>
            s.StartTimeUtc == proposedStartUtc &&
            s.IsAvailable);
    }

    private bool IsSlotTimeAvailable(
        TimeOnly slotStart,
        TimeOnly slotEnd,
        int bufferBeforeMinutes,
        int bufferAfterMinutes,
        List<(TimeOnly Start, TimeOnly End)> blockedRanges,
        List<Booking> existingBookings,
        DateOnly date,
        TimeZoneInfo hostTimezone)
    {
        // Calculate the full blocked period including buffers
        var blockedStart = slotStart.AddMinutes(-bufferBeforeMinutes);
        var blockedEnd = slotEnd.AddMinutes(bufferAfterMinutes);

        // Check against blocked time ranges from overrides
        foreach (var blocked in blockedRanges)
        {
            if (TimesOverlap(blockedStart, blockedEnd, blocked.Start, blocked.End))
            {
                return false;
            }
        }

        // Check against existing bookings
        foreach (var booking in existingBookings.Where(b => b.Status != BookingStatus.Cancelled))
        {
            // Convert booking time to local
            var bookingStartLocal = TimeZoneInfo.ConvertTimeFromUtc(
                booking.StartTimeUtc, hostTimezone);
            var bookingEndLocal = TimeZoneInfo.ConvertTimeFromUtc(
                booking.EndTimeUtc, hostTimezone);

            // Only check bookings on the same date
            if (DateOnly.FromDateTime(bookingStartLocal) != date)
                continue;

            var bookingStartTime = TimeOnly.FromDateTime(bookingStartLocal);
            var bookingEndTime = TimeOnly.FromDateTime(bookingEndLocal);

            if (TimesOverlap(blockedStart, blockedEnd, bookingStartTime, bookingEndTime))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TimesOverlap(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        // Two ranges overlap if one starts before the other ends AND ends after the other starts
        return start1 < end2 && end1 > start2;
    }

    private static TimeZoneInfo GetTimeZoneInfo(string timezone)
    {
        try
        {
            // Try IANA format first (cross-platform)
            return TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            // Try Windows format or fall back to UTC
            try
            {
                // Common mappings
                return timezone switch
                {
                    "America/New_York" => TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"),
                    "America/Chicago" => TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"),
                    "America/Denver" => TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"),
                    "America/Los_Angeles" => TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
                    "Europe/London" => TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"),
                    "Europe/Paris" => TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"),
                    "Asia/Tokyo" => TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"),
                    _ => TimeZoneInfo.Utc
                };
            }
            catch
            {
                return TimeZoneInfo.Utc;
            }
        }
    }
}
