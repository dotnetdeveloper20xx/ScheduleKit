using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Api.Models;
using ScheduleKit.Application.Commands.Users;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// API endpoints for user settings and profile management.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private MediatR.ISender? _mediator;

    protected MediatR.ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<MediatR.ISender>();

    /// <summary>
    /// Get current user's full profile with settings.
    /// </summary>
    /// <returns>Full user profile including settings.</returns>
    [HttpGet("me/profile")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile()
    {
        var currentUserService = HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        if (!currentUserService.IsAuthenticated)
            return Unauthorized();

        var userRepository = HttpContext.RequestServices
            .GetRequiredService<IUserRepository>();

        var user = await userRepository.GetByIdAsync(currentUserService.UserId);

        if (user == null)
            return Unauthorized();

        return Ok(new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Slug = user.Slug,
            Timezone = user.Timezone,
            EmailNotificationsEnabled = user.EmailNotificationsEnabled,
            ReminderEmailsEnabled = user.ReminderEmailsEnabled,
            ReminderHoursBefore = user.ReminderHoursBefore,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAtUtc
        });
    }

    /// <summary>
    /// Update current user's profile (name and slug).
    /// </summary>
    /// <param name="request">Profile update request.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("me/profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var currentUserService = HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        if (!currentUserService.IsAuthenticated)
            return Unauthorized();

        var command = new UpdateProfileCommand
        {
            UserId = currentUserService.UserId,
            Name = request.Name,
            Slug = request.Slug
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Profile Update Failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Update current user's timezone.
    /// </summary>
    /// <param name="request">Timezone update request.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("me/timezone")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateTimezone([FromBody] UpdateTimezoneRequest request)
    {
        var currentUserService = HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        if (!currentUserService.IsAuthenticated)
            return Unauthorized();

        var command = new UpdateTimezoneCommand
        {
            UserId = currentUserService.UserId,
            Timezone = request.Timezone
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Timezone Update Failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Update current user's email notification preferences.
    /// </summary>
    /// <param name="request">Email preferences update request.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("me/email-preferences")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmailPreferences([FromBody] UpdateEmailPreferencesRequest request)
    {
        var currentUserService = HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        if (!currentUserService.IsAuthenticated)
            return Unauthorized();

        var command = new UpdateEmailPreferencesCommand
        {
            UserId = currentUserService.UserId,
            EmailNotificationsEnabled = request.EmailNotificationsEnabled,
            ReminderEmailsEnabled = request.ReminderEmailsEnabled,
            ReminderHoursBefore = request.ReminderHoursBefore
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Email Preferences Update Failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Get a user's public profile by slug.
    /// </summary>
    /// <param name="slug">The user's public slug/username.</param>
    /// <returns>Public user profile.</returns>
    [HttpGet("{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicProfile(string slug)
    {
        var userRepository = HttpContext.RequestServices
            .GetRequiredService<IUserRepository>();

        var user = await userRepository.GetBySlugAsync(slug);

        if (user == null || !user.IsActive)
            return NotFound();

        return Ok(new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Slug = user.Slug,
            Timezone = user.Timezone
        });
    }
}
