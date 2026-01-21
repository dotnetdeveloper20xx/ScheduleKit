using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Mappings;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Queries.Bookings;

/// <summary>
/// Query to get a booking by ID.
/// </summary>
public record GetBookingByIdQuery : IQuery<BookingResponse>
{
    public Guid BookingId { get; init; }
    public Guid HostUserId { get; init; }
}

/// <summary>
/// Validator for GetBookingByIdQuery.
/// </summary>
public class GetBookingByIdQueryValidator : AbstractValidator<GetBookingByIdQuery>
{
    public GetBookingByIdQueryValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");
    }
}

/// <summary>
/// Handler for GetBookingByIdQuery.
/// </summary>
public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, Result<BookingResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventTypeRepository _eventTypeRepository;

    public GetBookingByIdQueryHandler(
        IBookingRepository bookingRepository,
        IEventTypeRepository eventTypeRepository)
    {
        _bookingRepository = bookingRepository;
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<Result<BookingResponse>> Handle(
        GetBookingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdWithResponsesAsync(
            request.BookingId, cancellationToken);

        if (booking == null)
            return Result.Failure<BookingResponse>("Booking not found.");

        // Verify ownership
        if (booking.HostUserId != request.HostUserId)
            return Result.Failure<BookingResponse>("Booking not found.");

        // Get event type name
        var eventType = await _eventTypeRepository.GetByIdAsync(
            booking.EventTypeId, cancellationToken);
        var eventTypeName = eventType?.Name ?? string.Empty;

        return Result.Success(booking.ToResponse(eventTypeName));
    }
}
