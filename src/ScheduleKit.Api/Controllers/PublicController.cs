using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Api.Models;
using ScheduleKit.Application.Commands.Bookings;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Queries.Availability;
using ScheduleKit.Application.Queries.EventTypes;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// Public API endpoints for guests to view event types and book appointments.
/// No authentication required.
/// </summary>
[ApiController]
[Route("api/v1/public")]
[Produces("application/json")]
public class PublicController : ControllerBase
{
    private MediatR.ISender? _mediator;

    protected MediatR.ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<MediatR.ISender>();

    /// <summary>
    /// Get a public event type by slug.
    /// </summary>
    /// <param name="hostSlug">The host's username/slug.</param>
    /// <param name="eventSlug">The event type slug.</param>
    /// <returns>Event type details if found and active.</returns>
    [HttpGet("{hostSlug}/{eventSlug}")]
    [ProducesResponseType(typeof(EventTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicEventType(string hostSlug, string eventSlug)
    {
        // TODO: Look up host by slug when user management is implemented
        // For now, we'll use the event slug directly with the test user

        var query = new GetEventTypeBySlugQuery
        {
            Slug = eventSlug
        };

        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = "Event type not found or not available for booking.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var eventType = result.Value;

        // Only return active event types
        if (!eventType.IsActive)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = "Event type is not currently accepting bookings.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(eventType);
    }

    /// <summary>
    /// Get available time slots for an event type on a specific date.
    /// </summary>
    /// <param name="eventTypeId">The event type ID.</param>
    /// <param name="date">The date (yyyy-MM-dd format).</param>
    /// <param name="timezone">The guest's timezone (IANA format, e.g., "America/New_York").</param>
    /// <returns>Available time slots.</returns>
    [HttpGet("slots/{eventTypeId:guid}")]
    [ProducesResponseType(typeof(AvailableSlotsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "date", "timezone" })]
    public async Task<IActionResult> GetAvailableSlots(
        Guid eventTypeId,
        [FromQuery] string date,
        [FromQuery] string timezone = "UTC")
    {
        var query = new GetAvailableSlotsQuery
        {
            EventTypeId = eventTypeId,
            Date = date,
            GuestTimezone = timezone
        };

        var result = await Mediator.Send(query);

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
    /// Get available dates for an event type within the booking window.
    /// </summary>
    /// <param name="eventTypeId">The event type ID.</param>
    /// <param name="timezone">The guest's timezone.</param>
    /// <returns>List of dates with availability status.</returns>
    [HttpGet("dates/{eventTypeId:guid}")]
    [ProducesResponseType(typeof(AvailableDatesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "timezone" })]
    public async Task<IActionResult> GetAvailableDates(
        Guid eventTypeId,
        [FromQuery] string timezone = "UTC")
    {
        var query = new GetAvailableDatesQuery
        {
            EventTypeId = eventTypeId,
            GuestTimezone = timezone
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
    /// Create a booking for a time slot.
    /// </summary>
    /// <param name="request">The booking details.</param>
    /// <returns>Booking confirmation details.</returns>
    [HttpPost("bookings")]
    [ProducesResponseType(typeof(BookingConfirmationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBooking([FromBody] CreatePublicBookingRequest request)
    {
        var command = new CreatePublicBookingCommand
        {
            EventTypeId = request.EventTypeId,
            GuestName = request.GuestName,
            GuestEmail = request.GuestEmail,
            GuestPhone = request.GuestPhone,
            GuestNotes = request.GuestNotes,
            StartTimeUtc = request.StartTimeUtc,
            GuestTimezone = request.GuestTimezone,
            QuestionResponses = request.QuestionResponses?
                .Select(q => new QuestionResponseDto
                {
                    QuestionId = q.QuestionId,
                    ResponseValue = q.ResponseValue
                })
                .ToList() ?? new List<QuestionResponseDto>()
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

        return CreatedAtAction(
            nameof(CreateBooking),
            new { id = result.Value.BookingId },
            result.Value);
    }
}
