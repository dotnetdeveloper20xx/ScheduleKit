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
/// Query to get bookings for a host with filtering and pagination.
/// </summary>
public record GetBookingsQuery : IQuery<PaginatedBookingsResponse>
{
    public Guid HostUserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

/// <summary>
/// Paginated response for bookings.
/// </summary>
public record PaginatedBookingsResponse
{
    public List<BookingResponse> Items { get; init; } = new();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}

/// <summary>
/// Validator for GetBookingsQuery.
/// </summary>
public class GetBookingsQueryValidator : AbstractValidator<GetBookingsQuery>
{
    public GetBookingsQueryValidator()
    {
        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.Status)
            .Must(BeValidStatus).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Invalid status. Must be one of: Confirmed, Cancelled, Completed, NoShow.");
    }

    private static bool BeValidStatus(string? status)
    {
        return Enum.TryParse<BookingStatus>(status, true, out _);
    }
}

/// <summary>
/// Handler for GetBookingsQuery.
/// </summary>
public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, Result<PaginatedBookingsResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventTypeRepository _eventTypeRepository;

    public GetBookingsQueryHandler(
        IBookingRepository bookingRepository,
        IEventTypeRepository eventTypeRepository)
    {
        _bookingRepository = bookingRepository;
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<Result<PaginatedBookingsResponse>> Handle(
        GetBookingsQuery request,
        CancellationToken cancellationToken)
    {
        // Parse status if provided
        BookingStatus? statusFilter = null;
        if (!string.IsNullOrEmpty(request.Status) &&
            Enum.TryParse<BookingStatus>(request.Status, true, out var parsed))
        {
            statusFilter = parsed;
        }

        // Get all bookings matching the filter
        var allBookings = await _bookingRepository.GetByHostUserIdAsync(
            request.HostUserId,
            request.FromDate,
            request.ToDate,
            statusFilter,
            cancellationToken);

        // Sort by start time descending (most recent first)
        var sortedBookings = allBookings
            .OrderByDescending(b => b.StartTimeUtc)
            .ToList();

        // Calculate pagination
        var totalCount = sortedBookings.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        // Get page items
        var pagedBookings = sortedBookings
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Get event type names for the bookings
        var eventTypeIds = pagedBookings.Select(b => b.EventTypeId).Distinct().ToList();
        var eventTypes = new Dictionary<Guid, string>();

        foreach (var eventTypeId in eventTypeIds)
        {
            var eventType = await _eventTypeRepository.GetByIdAsync(eventTypeId, cancellationToken);
            if (eventType != null)
            {
                eventTypes[eventTypeId] = eventType.Name;
            }
        }

        return Result.Success(new PaginatedBookingsResponse
        {
            Items = pagedBookings.ToResponseList(eventTypes),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        });
    }
}
