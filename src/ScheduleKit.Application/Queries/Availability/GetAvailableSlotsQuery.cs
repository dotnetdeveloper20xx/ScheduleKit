using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Domain.Services;

namespace ScheduleKit.Application.Queries.Availability;

/// <summary>
/// Query to get available time slots for a specific event type and date.
/// Used by the public booking page.
/// </summary>
public record GetAvailableSlotsQuery : IRequest<Result<AvailableSlotsResponse>>
{
    public Guid EventTypeId { get; init; }
    public string Date { get; init; } = string.Empty;
    public string GuestTimezone { get; init; } = "UTC";
}

/// <summary>
/// Validator for GetAvailableSlotsQuery.
/// </summary>
public class GetAvailableSlotsQueryValidator : AbstractValidator<GetAvailableSlotsQuery>
{
    public GetAvailableSlotsQueryValidator()
    {
        RuleFor(x => x.EventTypeId)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Date must be in yyyy-MM-dd format.");

        RuleFor(x => x.GuestTimezone)
            .NotEmpty().WithMessage("Timezone is required.");
    }
}

/// <summary>
/// Handler for GetAvailableSlotsQuery.
/// </summary>
public class GetAvailableSlotsQueryHandler
    : IRequestHandler<GetAvailableSlotsQuery, Result<AvailableSlotsResponse>>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IAvailabilityOverrideRepository _overrideRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ISlotCalculator _slotCalculator;

    public GetAvailableSlotsQueryHandler(
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

    public async Task<Result<AvailableSlotsResponse>> Handle(
        GetAvailableSlotsQuery request,
        CancellationToken cancellationToken)
    {
        // Parse date
        if (!DateOnly.TryParse(request.Date, out var date))
            return Result.Failure<AvailableSlotsResponse>("Invalid date format.");

        // Get event type
        var eventType = await _eventTypeRepository.GetByIdAsync(request.EventTypeId, cancellationToken);
        if (eventType == null)
            return Result.Failure<AvailableSlotsResponse>("Event type not found.");

        if (!eventType.IsActive)
            return Result.Failure<AvailableSlotsResponse>("This event type is not currently accepting bookings.");

        // Check booking window (default: 60 days)
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var bookingWindowDays = 60; // TODO: Add BookingWindow value object to EventType
        var maxBookingDate = today.AddDays(bookingWindowDays);

        if (date < today)
            return Result.Failure<AvailableSlotsResponse>("Cannot get slots for past dates.");

        if (date > maxBookingDate)
            return Result.Failure<AvailableSlotsResponse>(
                $"Cannot book more than {bookingWindowDays} days in advance.");

        // Get host's availability
        var availabilities = await _availabilityRepository.GetByHostUserIdAsync(
            eventType.HostUserId, cancellationToken);

        // Get overrides for the date
        var overrides = await _overrideRepository.GetByHostUserIdAsync(
            eventType.HostUserId,
            date,
            date,
            cancellationToken);

        // Get existing bookings for the date
        var dateStartUtc = date.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        var dateEndUtc = date.ToDateTime(TimeOnly.MaxValue).ToUniversalTime();

        var existingBookings = await _bookingRepository.GetByHostUserIdAsync(
            eventType.HostUserId,
            dateStartUtc,
            dateEndUtc,
            null,
            cancellationToken);

        // Calculate slots (using host timezone - UTC for now)
        var hostTimezone = "UTC"; // TODO: Get from user preferences

        var calculatedSlots = _slotCalculator.CalculateSlotsForDate(
            eventType,
            availabilities,
            overrides,
            existingBookings,
            date,
            hostTimezone);

        // Convert to response format with guest timezone
        var guestTz = GetTimeZoneInfo(request.GuestTimezone);
        var slots = calculatedSlots
            .Where(s => s.IsAvailable)
            .Select(s =>
            {
                var guestStart = TimeZoneInfo.ConvertTimeFromUtc(s.StartTimeUtc, guestTz);
                var guestEnd = TimeZoneInfo.ConvertTimeFromUtc(s.EndTimeUtc, guestTz);

                return new TimeSlotResponse
                {
                    StartTime = guestStart.ToString("HH:mm"),
                    EndTime = guestEnd.ToString("HH:mm"),
                    StartTimeUtc = s.StartTimeUtc,
                    EndTimeUtc = s.EndTimeUtc,
                    IsAvailable = true
                };
            })
            .ToList();

        return Result.Success(new AvailableSlotsResponse
        {
            Date = date.ToString("yyyy-MM-dd"),
            Timezone = request.GuestTimezone,
            EventTypeId = eventType.Id,
            Slots = slots
        });
    }

    private static TimeZoneInfo GetTimeZoneInfo(string timezone)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
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
