using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Application.Common.Mappings;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Domain.Services;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Application.Commands.Bookings;

/// <summary>
/// Command to reschedule a booking to a new time slot.
/// </summary>
public record RescheduleBookingCommand : ICommand<BookingResponse>
{
    public Guid BookingId { get; init; }
    public string? RescheduleToken { get; init; }
    public Guid? HostUserId { get; init; }
    public DateTime NewStartTimeUtc { get; init; }
}

/// <summary>
/// Validator for RescheduleBookingCommand.
/// </summary>
public class RescheduleBookingCommandValidator : AbstractValidator<RescheduleBookingCommand>
{
    public RescheduleBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.NewStartTimeUtc)
            .NotEmpty().WithMessage("New start time is required.")
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Cannot reschedule to a past time.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.RescheduleToken) || x.HostUserId.HasValue)
            .WithMessage("Either reschedule token or host user ID must be provided.");
    }
}

/// <summary>
/// Handler for RescheduleBookingCommand.
/// </summary>
public class RescheduleBookingCommandHandler
    : IRequestHandler<RescheduleBookingCommand, Result<BookingResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IAvailabilityOverrideRepository _overrideRepository;
    private readonly ISlotCalculator _slotCalculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealTimeNotificationService _notificationService;

    public RescheduleBookingCommandHandler(
        IBookingRepository bookingRepository,
        IEventTypeRepository eventTypeRepository,
        IAvailabilityRepository availabilityRepository,
        IAvailabilityOverrideRepository overrideRepository,
        ISlotCalculator slotCalculator,
        IUnitOfWork unitOfWork,
        IRealTimeNotificationService notificationService)
    {
        _bookingRepository = bookingRepository;
        _eventTypeRepository = eventTypeRepository;
        _availabilityRepository = availabilityRepository;
        _overrideRepository = overrideRepository;
        _slotCalculator = slotCalculator;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<BookingResponse>> Handle(
        RescheduleBookingCommand request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if (booking == null)
            return Result.Failure<BookingResponse>("Booking not found.");

        // Verify authorization
        if (request.HostUserId.HasValue)
        {
            // Host is rescheduling
            if (booking.HostUserId != request.HostUserId.Value)
                return Result.Failure<BookingResponse>("Booking not found.");
        }
        else if (!string.IsNullOrEmpty(request.RescheduleToken))
        {
            // Guest is rescheduling with token
            if (!booking.ValidateRescheduleToken(request.RescheduleToken))
                return Result.Failure<BookingResponse>("Invalid reschedule token.");
        }
        else
        {
            return Result.Failure<BookingResponse>("Unauthorized.");
        }

        // Get event type for duration
        var eventType = await _eventTypeRepository.GetByIdAsync(
            booking.EventTypeId, cancellationToken);

        if (eventType == null)
            return Result.Failure<BookingResponse>("Event type not found.");

        // Validate the new slot is available
        var date = DateOnly.FromDateTime(request.NewStartTimeUtc);
        var availabilities = await _availabilityRepository.GetByHostUserIdAsync(
            booking.HostUserId, cancellationToken);
        var overrides = await _overrideRepository.GetByHostUserIdAsync(
            booking.HostUserId, date, date, cancellationToken);
        var existingBookings = await _bookingRepository.GetByEventTypeIdAsync(
            booking.EventTypeId,
            request.NewStartTimeUtc.AddHours(-24),
            request.NewStartTimeUtc.AddHours(24),
            cancellationToken);

        // Exclude the current booking from conflict check
        existingBookings = existingBookings.Where(b => b.Id != booking.Id).ToList();

        var hostTimezone = "UTC"; // TODO: Get from user preferences
        var slots = _slotCalculator.CalculateSlotsForDate(
            eventType, availabilities, overrides, existingBookings, date, hostTimezone);

        var matchingSlot = slots.FirstOrDefault(s =>
            Math.Abs((s.StartTimeUtc - request.NewStartTimeUtc).TotalMinutes) < 2);

        if (matchingSlot == null)
            return Result.Failure<BookingResponse>(
                "The selected time slot is not available.");

        // Store old time for notifications
        var oldStartTimeUtc = booking.StartTimeUtc;

        // Create new time slot
        var newTimeSlotResult = TimeSlot.Create(request.NewStartTimeUtc, eventType.Duration);
        if (newTimeSlotResult.IsFailure)
            return Result.Failure<BookingResponse>(newTimeSlotResult.Error);

        // Reschedule the booking
        var rescheduleResult = booking.Reschedule(newTimeSlotResult.Value, eventType.Duration);
        if (rescheduleResult.IsFailure)
            return Result.Failure<BookingResponse>(rescheduleResult.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send real-time notifications
        await _notificationService.NotifyBookingRescheduledAsync(
            booking.HostUserId,
            booking.EventTypeId,
            booking.Id,
            oldStartTimeUtc,
            booking.StartTimeUtc,
            cancellationToken);

        return Result.Success(booking.ToResponse(eventType.Name));
    }
}
