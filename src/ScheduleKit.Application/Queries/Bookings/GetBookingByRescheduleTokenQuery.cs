using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Mappings;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Queries.Bookings;

/// <summary>
/// Response DTO for reschedule booking lookup.
/// </summary>
public record RescheduleBookingInfoResponse
{
    public Guid BookingId { get; init; }
    public Guid EventTypeId { get; init; }
    public string EventTypeName { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public string GuestName { get; init; } = string.Empty;
    public string GuestEmail { get; init; } = string.Empty;
    public DateTime CurrentStartTimeUtc { get; init; }
    public DateTime CurrentEndTimeUtc { get; init; }
    public string GuestTimezone { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string LocationType { get; init; } = string.Empty;
    public string? LocationDetails { get; init; }
    public string? LocationDisplayName { get; init; }
}

/// <summary>
/// Query to get booking information by reschedule token.
/// </summary>
public record GetBookingByRescheduleTokenQuery : IQuery<RescheduleBookingInfoResponse>
{
    public string Token { get; init; } = string.Empty;
}

/// <summary>
/// Validator for GetBookingByRescheduleTokenQuery.
/// </summary>
public class GetBookingByRescheduleTokenQueryValidator : AbstractValidator<GetBookingByRescheduleTokenQuery>
{
    public GetBookingByRescheduleTokenQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Reschedule token is required.");
    }
}

/// <summary>
/// Handler for GetBookingByRescheduleTokenQuery.
/// </summary>
public class GetBookingByRescheduleTokenQueryHandler
    : IRequestHandler<GetBookingByRescheduleTokenQuery, Result<RescheduleBookingInfoResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventTypeRepository _eventTypeRepository;

    public GetBookingByRescheduleTokenQueryHandler(
        IBookingRepository bookingRepository,
        IEventTypeRepository eventTypeRepository)
    {
        _bookingRepository = bookingRepository;
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<Result<RescheduleBookingInfoResponse>> Handle(
        GetBookingByRescheduleTokenQuery request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByRescheduleTokenAsync(
            request.Token, cancellationToken);

        if (booking == null)
        {
            return Result.Failure<RescheduleBookingInfoResponse>(
                "Booking not found or reschedule link has expired.");
        }

        // Can only reschedule confirmed bookings
        if (booking.Status != BookingStatus.Confirmed)
        {
            return Result.Failure<RescheduleBookingInfoResponse>(
                "This booking cannot be rescheduled.");
        }

        // Can only reschedule future bookings
        if (booking.StartTimeUtc <= DateTime.UtcNow)
        {
            return Result.Failure<RescheduleBookingInfoResponse>(
                "Cannot reschedule a booking that has already passed.");
        }

        var eventType = await _eventTypeRepository.GetByIdAsync(
            booking.EventTypeId, cancellationToken);

        if (eventType == null)
        {
            return Result.Failure<RescheduleBookingInfoResponse>(
                "Event type not found.");
        }

        return Result.Success(new RescheduleBookingInfoResponse
        {
            BookingId = booking.Id,
            EventTypeId = booking.EventTypeId,
            EventTypeName = eventType.Name,
            DurationMinutes = eventType.Duration.Minutes,
            GuestName = booking.GuestName,
            GuestEmail = booking.GuestEmail,
            CurrentStartTimeUtc = booking.StartTimeUtc,
            CurrentEndTimeUtc = booking.EndTimeUtc,
            GuestTimezone = booking.GuestTimezone,
            Status = booking.Status.ToString(),
            LocationType = eventType.Location.Type.ToString(),
            LocationDetails = eventType.Location.Details,
            LocationDisplayName = eventType.Location.DisplayName
        });
    }
}
