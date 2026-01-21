using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Api.Models;
using ScheduleKit.Application.Commands.Bookings;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Application.Queries.Bookings;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// API endpoints for managing bookings.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private MediatR.ISender? _mediator;

    protected MediatR.ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<MediatR.ISender>();

    // TODO: Get from authenticated user context
    private static readonly Guid TestHostUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>
    /// Get a paginated list of bookings for the authenticated host.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100).</param>
    /// <param name="status">Filter by status (Confirmed, Cancelled, Completed, NoShow).</param>
    /// <param name="fromDate">Filter bookings starting from this date.</param>
    /// <param name="toDate">Filter bookings up to this date.</param>
    /// <returns>Paginated list of bookings.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedBookingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBookings(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = new GetBookingsQuery
        {
            HostUserId = TestHostUserId,
            Page = page,
            PageSize = pageSize,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a specific booking by ID.
    /// </summary>
    /// <param name="id">The booking ID.</param>
    /// <returns>The booking details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var query = new GetBookingByIdQuery
        {
            BookingId = id,
            HostUserId = TestHostUserId
        };

        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cancel a booking.
    /// </summary>
    /// <param name="id">The booking ID.</param>
    /// <param name="request">Cancellation details.</param>
    /// <returns>No content if successful.</returns>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelBooking(Guid id, [FromBody] CancelBookingRequest? request)
    {
        var command = new CancelBookingCommand
        {
            BookingId = id,
            RequestedByUserId = TestHostUserId,
            IsHost = true,
            Reason = request?.Reason
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Reschedule a booking to a new time slot.
    /// </summary>
    /// <param name="id">The booking ID.</param>
    /// <param name="request">The new time details.</param>
    /// <returns>The updated booking.</returns>
    [HttpPost("{id:guid}/reschedule")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RescheduleBooking(Guid id, [FromBody] RescheduleBookingRequest request)
    {
        var command = new RescheduleBookingCommand
        {
            BookingId = id,
            HostUserId = TestHostUserId,
            NewStartTimeUtc = request.NewStartTimeUtc
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Download an ICS calendar file for a booking.
    /// </summary>
    /// <param name="id">The booking ID.</param>
    /// <returns>ICS calendar file.</returns>
    [HttpGet("{id:guid}/calendar.ics")]
    [Produces("text/calendar")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadCalendar(Guid id)
    {
        var query = new GetBookingByIdQuery
        {
            BookingId = id,
            HostUserId = TestHostUserId
        };

        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        var booking = result.Value;
        var calendarService = HttpContext.RequestServices.GetRequiredService<ICalendarService>();

        var icsContent = calendarService.GenerateIcsForBooking(new CalendarEventData
        {
            BookingId = id,
            EventTitle = booking.EventTypeName,
            Description = $"Meeting with {booking.GuestName}",
            StartTimeUtc = booking.StartTimeUtc,
            EndTimeUtc = booking.EndTimeUtc,
            Location = booking.MeetingLink,
            OrganizerName = "Host", // TODO: Get from user profile
            OrganizerEmail = "host@schedulekit.com", // TODO: Get from user profile
            AttendeeName = booking.GuestName,
            AttendeeEmail = booking.GuestEmail
        });

        var bytes = System.Text.Encoding.UTF8.GetBytes(icsContent);
        return File(bytes, "text/calendar", $"booking-{id}.ics");
    }
}
