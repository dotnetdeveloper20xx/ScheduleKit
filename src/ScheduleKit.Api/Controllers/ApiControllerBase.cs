using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Domain.Common;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// Base controller providing common functionality for all API controllers.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// Converts a Result to an appropriate ActionResult.
    /// </summary>
    protected IActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(new ProblemDetails
        {
            Title = "Request Failed",
            Detail = result.Error,
            Status = StatusCodes.Status400BadRequest
        });
    }

    /// <summary>
    /// Converts a Result{T} to an appropriate ActionResult.
    /// </summary>
    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new ProblemDetails
        {
            Title = "Request Failed",
            Detail = result.Error,
            Status = StatusCodes.Status400BadRequest
        });
    }

    /// <summary>
    /// Converts a Result{T} to an appropriate ActionResult for creation operations.
    /// </summary>
    protected IActionResult ToCreatedResult<T>(Result<T> result, string actionName, object routeValues)
    {
        if (result.IsSuccess)
        {
            return CreatedAtAction(actionName, routeValues, result.Value);
        }

        return BadRequest(new ProblemDetails
        {
            Title = "Request Failed",
            Detail = result.Error,
            Status = StatusCodes.Status400BadRequest
        });
    }

    /// <summary>
    /// Gets the current user ID from claims.
    /// For now, returns a test user ID. Will be replaced with actual auth.
    /// </summary>
    protected Guid GetCurrentUserId()
    {
        // TODO: Replace with actual user ID from claims when auth is implemented
        // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        // return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;

        // For development/testing, use a consistent test user ID
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
