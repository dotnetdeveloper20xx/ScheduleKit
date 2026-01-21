using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Api.Models;
using ScheduleKit.Application.Commands.Availability;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Queries.Availability;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// API endpoints for managing host availability.
/// </summary>
public class AvailabilityController : ApiControllerBase
{
    /// <summary>
    /// Get the current host's weekly availability schedule.
    /// </summary>
    /// <returns>Weekly availability schedule.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(WeeklyAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWeeklyAvailability()
    {
        var query = new GetWeeklyAvailabilityQuery
        {
            HostUserId = GetCurrentUserId()
        };

        var result = await Mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Update the current host's weekly availability schedule.
    /// </summary>
    /// <param name="request">The updated availability schedule.</param>
    /// <returns>The updated availability schedule.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(WeeklyAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateWeeklyAvailability([FromBody] UpdateWeeklyAvailabilityRequest request)
    {
        var command = new UpdateWeeklyAvailabilityCommand
        {
            HostUserId = GetCurrentUserId(),
            Days = request.Days.Select(d => new DayAvailabilityUpdate
            {
                DayOfWeek = d.DayOfWeek,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                IsEnabled = d.IsEnabled
            }).ToList()
        };

        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get the current host's availability overrides.
    /// </summary>
    /// <param name="fromDate">Optional start date filter (yyyy-MM-dd).</param>
    /// <param name="toDate">Optional end date filter (yyyy-MM-dd).</param>
    /// <returns>List of availability overrides.</returns>
    [HttpGet("overrides")]
    [ProducesResponseType(typeof(List<AvailabilityOverrideResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOverrides(
        [FromQuery] string? fromDate = null,
        [FromQuery] string? toDate = null)
    {
        DateOnly? from = null;
        DateOnly? to = null;

        if (!string.IsNullOrEmpty(fromDate) && DateOnly.TryParse(fromDate, out var fromParsed))
            from = fromParsed;

        if (!string.IsNullOrEmpty(toDate) && DateOnly.TryParse(toDate, out var toParsed))
            to = toParsed;

        var query = new GetAvailabilityOverridesQuery
        {
            HostUserId = GetCurrentUserId(),
            FromDate = from,
            ToDate = to
        };

        var result = await Mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Create a new availability override (block a day/time or add extra availability).
    /// </summary>
    /// <param name="request">The override details.</param>
    /// <returns>The created override.</returns>
    [HttpPost("overrides")]
    [ProducesResponseType(typeof(AvailabilityOverrideResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOverride([FromBody] CreateAvailabilityOverrideRequest request)
    {
        var command = new CreateAvailabilityOverrideCommand
        {
            HostUserId = GetCurrentUserId(),
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsBlocked = request.IsBlocked,
            Reason = request.Reason
        };

        var result = await Mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetOverrides),
                new { },
                result.Value);
        }

        return ToActionResult(result);
    }

    /// <summary>
    /// Delete an availability override.
    /// </summary>
    /// <param name="id">The override ID.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("overrides/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOverride(Guid id)
    {
        var command = new DeleteAvailabilityOverrideCommand
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
