using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Application.Common.Mappings;

/// <summary>
/// Extension methods for mapping Booking entities to DTOs.
/// </summary>
public static class BookingMappings
{
    public static BookingResponse ToResponse(this Booking booking, string? eventTypeName = null)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            EventTypeId = booking.EventTypeId,
            EventTypeName = eventTypeName ?? booking.EventType?.Name ?? string.Empty,
            HostUserId = booking.HostUserId,
            GuestName = booking.GuestName,
            GuestEmail = booking.GuestEmail,
            GuestPhone = booking.GuestPhone,
            GuestNotes = booking.GuestNotes,
            StartTimeUtc = booking.StartTimeUtc,
            EndTimeUtc = booking.EndTimeUtc,
            GuestTimezone = booking.GuestTimezone,
            Status = booking.Status.ToString(),
            CancellationReason = booking.CancellationReason,
            CancelledAtUtc = booking.CancelledAtUtc,
            MeetingLink = booking.MeetingLink,
            CreatedAtUtc = booking.CreatedAtUtc,
            Responses = booking.Responses.Select(r => r.ToResponse()).ToList()
        };
    }

    public static BookingQuestionAnswerResponse ToResponse(this Domain.Entities.BookingQuestionResponse response)
    {
        return new BookingQuestionAnswerResponse
        {
            QuestionId = response.QuestionId,
            QuestionText = string.Empty, // Would need to join with question
            ResponseValue = response.ResponseValue
        };
    }

    public static List<BookingResponse> ToResponseList(
        this IEnumerable<Booking> bookings,
        Dictionary<Guid, string>? eventTypeNames = null)
    {
        return bookings.Select(b => b.ToResponse(
            eventTypeNames?.GetValueOrDefault(b.EventTypeId)
        )).ToList();
    }
}
