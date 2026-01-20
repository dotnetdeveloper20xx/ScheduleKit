using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Api.Models;
using ScheduleKit.Application.Commands.EventTypes;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Queries.EventTypes;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// API endpoints for managing event types.
/// </summary>
public class EventTypesController : ApiControllerBase
{
    /// <summary>
    /// Get all event types for the current host user.
    /// </summary>
    /// <returns>List of event types.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<EventTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventTypes()
    {
        var query = new GetEventTypesQuery
        {
            HostUserId = GetCurrentUserId()
        };

        var result = await Mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get a specific event type by ID.
    /// </summary>
    /// <param name="id">The event type ID.</param>
    /// <returns>The event type details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventType(Guid id)
    {
        var query = new GetEventTypeByIdQuery
        {
            Id = id,
            HostUserId = GetCurrentUserId()
        };

        var result = await Mediator.Send(query);

        if (result.IsFailure && result.Error.Contains("not found"))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return ToActionResult(result);
    }

    /// <summary>
    /// Create a new event type.
    /// </summary>
    /// <param name="request">The event type details.</param>
    /// <returns>The created event type.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(EventTypeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEventType([FromBody] CreateEventTypeRequest request)
    {
        var command = new CreateEventTypeCommand
        {
            HostUserId = GetCurrentUserId(),
            Name = request.Name,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            BufferBeforeMinutes = request.BufferBeforeMinutes,
            BufferAfterMinutes = request.BufferAfterMinutes,
            LocationType = request.LocationType,
            LocationDetails = request.LocationDetails,
            Color = request.Color
        };

        var result = await Mediator.Send(command);
        return ToCreatedResult(result, nameof(GetEventType), new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    /// <summary>
    /// Update an existing event type.
    /// </summary>
    /// <param name="id">The event type ID.</param>
    /// <param name="request">The updated event type details.</param>
    /// <returns>The updated event type.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EventTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEventType(Guid id, [FromBody] UpdateEventTypeRequest request)
    {
        var command = new UpdateEventTypeCommand
        {
            Id = id,
            HostUserId = GetCurrentUserId(),
            Name = request.Name,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            BufferBeforeMinutes = request.BufferBeforeMinutes,
            BufferAfterMinutes = request.BufferAfterMinutes,
            LocationType = request.LocationType,
            LocationDetails = request.LocationDetails,
            Color = request.Color
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure && result.Error.Contains("not found"))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return ToActionResult(result);
    }

    /// <summary>
    /// Delete an event type.
    /// </summary>
    /// <param name="id">The event type ID.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEventType(Guid id)
    {
        var command = new DeleteEventTypeCommand
        {
            Id = id,
            HostUserId = GetCurrentUserId()
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure && result.Error.Contains("not found"))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return ToActionResult(result);
    }
}
