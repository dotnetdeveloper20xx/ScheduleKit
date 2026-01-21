using System.Text;
using ScheduleKit.Application.Common.Interfaces;

namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// Service for generating ICS calendar files.
/// </summary>
public class CalendarService : ICalendarService
{
    public string GenerateIcsForBooking(CalendarEventData data)
    {
        var sb = new StringBuilder();

        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//ScheduleKit//Booking//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:REQUEST");

        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{data.BookingId}@schedulekit.com");
        sb.AppendLine($"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");
        sb.AppendLine($"DTSTART:{FormatDateTime(data.StartTimeUtc)}");
        sb.AppendLine($"DTEND:{FormatDateTime(data.EndTimeUtc)}");
        sb.AppendLine($"SUMMARY:{EscapeText(data.EventTitle)}");

        if (!string.IsNullOrEmpty(data.Description))
        {
            sb.AppendLine($"DESCRIPTION:{EscapeText(data.Description)}");
        }

        if (!string.IsNullOrEmpty(data.Location))
        {
            sb.AppendLine($"LOCATION:{EscapeText(data.Location)}");
        }

        sb.AppendLine($"ORGANIZER;CN={EscapeText(data.OrganizerName)}:mailto:{data.OrganizerEmail}");
        sb.AppendLine($"ATTENDEE;CN={EscapeText(data.AttendeeName)};RSVP=TRUE;PARTSTAT=NEEDS-ACTION:mailto:{data.AttendeeEmail}");

        sb.AppendLine($"SEQUENCE:{data.SequenceNumber}");
        sb.AppendLine("STATUS:CONFIRMED");

        // Add reminder 15 minutes before
        sb.AppendLine("BEGIN:VALARM");
        sb.AppendLine("TRIGGER:-PT15M");
        sb.AppendLine("ACTION:DISPLAY");
        sb.AppendLine($"DESCRIPTION:Reminder: {EscapeText(data.EventTitle)}");
        sb.AppendLine("END:VALARM");

        // Add reminder 1 hour before
        sb.AppendLine("BEGIN:VALARM");
        sb.AppendLine("TRIGGER:-PT1H");
        sb.AppendLine("ACTION:DISPLAY");
        sb.AppendLine($"DESCRIPTION:Reminder: {EscapeText(data.EventTitle)} in 1 hour");
        sb.AppendLine("END:VALARM");

        sb.AppendLine("END:VEVENT");
        sb.AppendLine("END:VCALENDAR");

        return sb.ToString();
    }

    public string GenerateIcsCancellation(CalendarEventData data)
    {
        var sb = new StringBuilder();

        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//ScheduleKit//Booking//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:CANCEL");

        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{data.BookingId}@schedulekit.com");
        sb.AppendLine($"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");
        sb.AppendLine($"DTSTART:{FormatDateTime(data.StartTimeUtc)}");
        sb.AppendLine($"DTEND:{FormatDateTime(data.EndTimeUtc)}");
        sb.AppendLine($"SUMMARY:{EscapeText(data.EventTitle)} (CANCELLED)");

        if (!string.IsNullOrEmpty(data.Description))
        {
            sb.AppendLine($"DESCRIPTION:{EscapeText(data.Description)}");
        }

        sb.AppendLine($"ORGANIZER;CN={EscapeText(data.OrganizerName)}:mailto:{data.OrganizerEmail}");
        sb.AppendLine($"ATTENDEE;CN={EscapeText(data.AttendeeName)}:mailto:{data.AttendeeEmail}");

        sb.AppendLine($"SEQUENCE:{data.SequenceNumber + 1}");
        sb.AppendLine("STATUS:CANCELLED");

        sb.AppendLine("END:VEVENT");
        sb.AppendLine("END:VCALENDAR");

        return sb.ToString();
    }

    private static string FormatDateTime(DateTime dateTime)
    {
        // Format: YYYYMMDDTHHMMSSZ (UTC)
        return dateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
    }

    private static string EscapeText(string text)
    {
        // Escape special characters in ICS
        return text
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\n", "\\n")
            .Replace("\r", "");
    }
}
