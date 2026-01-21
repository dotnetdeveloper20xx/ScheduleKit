using MediatR;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Domain.Services;

namespace ScheduleKit.Application.Queries.Availability;

/// <summary>
/// Query to get dates with availability within the booking window.
/// </summary>
public record GetAvailableDatesQuery : IRequest<Result<AvailableDatesResponse>>
{
    public Guid EventTypeId { get; init; }
    public string GuestTimezone { get; init; } = "UTC";
}

/// <summary>
/// Handler for GetAvailableDatesQuery.
/// </summary>
public class GetAvailableDatesQueryHandler
    : IRequestHandler<GetAvailableDatesQuery, Result<AvailableDatesResponse>>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IAvailabilityOverrideRepository _overrideRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ISlotCalculator _slotCalculator;

    public GetAvailableDatesQueryHandler(
        IEventTypeRepository eventTypeRepository,
        IAvailabilityRepository availabilityRepository,
        IAvailabilityOverrideRepository overrideRepository,
        IBookingRepository bookingRepository,
        ISlotCalculator slotCalculator)
    {
        _eventTypeRepository = eventTypeRepository;
        _availabilityRepository = availabilityRepository;
        _overrideRepository = overrideRepository;
        _bookingRepository = bookingRepository;
        _slotCalculator = slotCalculator;
    }

    public async Task<Result<AvailableDatesResponse>> Handle(
        GetAvailableDatesQuery request,
        CancellationToken cancellationToken)
    {
        // Get event type
        var eventType = await _eventTypeRepository.GetByIdAsync(request.EventTypeId, cancellationToken);
        if (eventType == null)
            return Result.Failure<AvailableDatesResponse>("Event type not found.");

        if (!eventType.IsActive)
            return Result.Failure<AvailableDatesResponse>("This event type is not currently accepting bookings.");

        // Calculate date range (default: 60 days booking window)
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var bookingWindowDays = 60; // TODO: Add BookingWindow value object to EventType
        var endDate = today.AddDays(bookingWindowDays);

        // Get host's availability
        var availabilities = await _availabilityRepository.GetByHostUserIdAsync(
            eventType.HostUserId, cancellationToken);

        // Get overrides for the entire period
        var overrides = await _overrideRepository.GetByHostUserIdAsync(
            eventType.HostUserId,
            today,
            endDate,
            cancellationToken);

        // Get existing bookings for the period
        var startUtc = today.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        var endUtc = endDate.ToDateTime(TimeOnly.MaxValue).ToUniversalTime();

        var existingBookings = await _bookingRepository.GetByHostUserIdAsync(
            eventType.HostUserId,
            startUtc,
            endUtc,
            null,
            cancellationToken);

        // Calculate availability for each date
        var hostTimezone = "UTC";
        var dates = new List<DateAvailabilityResponse>();

        for (var date = today; date <= endDate; date = date.AddDays(1))
        {
            var dayOverrides = overrides.Where(o => o.Date == date).ToList();
            var dayBookings = existingBookings
                .Where(b => DateOnly.FromDateTime(b.StartTimeUtc) == date)
                .ToList();

            var slots = _slotCalculator.CalculateSlotsForDate(
                eventType,
                availabilities,
                dayOverrides,
                dayBookings,
                date,
                hostTimezone);

            var availableSlots = slots.Count(s => s.IsAvailable);

            dates.Add(new DateAvailabilityResponse
            {
                Date = date.ToString("yyyy-MM-dd"),
                DayOfWeek = date.DayOfWeek,
                HasAvailability = availableSlots > 0,
                AvailableSlotCount = availableSlots
            });
        }

        return Result.Success(new AvailableDatesResponse
        {
            EventTypeId = eventType.Id,
            FromDate = today.ToString("yyyy-MM-dd"),
            ToDate = endDate.ToString("yyyy-MM-dd"),
            Timezone = request.GuestTimezone,
            Dates = dates
        });
    }
}
