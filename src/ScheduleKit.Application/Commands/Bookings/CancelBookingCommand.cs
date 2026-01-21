using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Events;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Bookings;

/// <summary>
/// Command to cancel a booking.
/// </summary>
public record CancelBookingCommand : ICommand
{
    public Guid BookingId { get; init; }
    public Guid RequestedByUserId { get; init; }
    public bool IsHost { get; init; }
    public string? Reason { get; init; }
}

/// <summary>
/// Validator for CancelBookingCommand.
/// </summary>
public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Cancellation reason must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}

/// <summary>
/// Handler for CancelBookingCommand.
/// </summary>
public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealTimeNotificationService _notificationService;

    public CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IRealTimeNotificationService notificationService)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if (booking == null)
            return Result.Failure("Booking not found.");

        // Verify ownership - host can cancel any booking, guest would need email verification
        if (request.IsHost && booking.HostUserId != request.RequestedByUserId)
            return Result.Failure("Booking not found.");

        // Store values before cancellation for notifications
        var eventTypeId = booking.EventTypeId;
        var startTimeUtc = booking.StartTimeUtc;
        var hostUserId = booking.HostUserId;

        var cancelledBy = request.IsHost ? CancelledBy.Host : CancelledBy.Guest;
        var cancelResult = booking.Cancel(request.Reason, cancelledBy);

        if (cancelResult.IsFailure)
            return cancelResult;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send real-time notifications
        await _notificationService.NotifySlotReleasedAsync(
            eventTypeId, startTimeUtc, cancellationToken);

        await _notificationService.NotifyBookingCancelledAsync(
            hostUserId, request.BookingId, request.Reason, cancellationToken);

        return Result.Success();
    }
}
