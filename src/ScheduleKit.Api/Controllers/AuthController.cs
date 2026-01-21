using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Api.Models;
using ScheduleKit.Application.Commands.Auth;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.Interfaces;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// API endpoints for authentication.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private MediatR.ISender? _mediator;

    protected MediatR.ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<MediatR.ISender>();

    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <param name="request">Registration details.</param>
    /// <returns>Authentication result with tokens.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand
        {
            Email = request.Email,
            Password = request.Password,
            Name = request.Name,
            Slug = request.Slug
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Registration Failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        var authResult = result.Value;
        return Ok(new AuthResponse
        {
            AccessToken = authResult.AccessToken!,
            RefreshToken = authResult.RefreshToken!,
            ExpiresAt = authResult.ExpiresAt!.Value,
            User = new UserResponse
            {
                Id = authResult.User!.Id,
                Email = authResult.User.Email,
                Name = authResult.User.Name,
                Slug = authResult.User.Slug,
                Timezone = authResult.User.Timezone
            }
        });
    }

    /// <summary>
    /// Login with email and password.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>Authentication result with tokens.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Login Failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var authResult = result.Value;
        return Ok(new AuthResponse
        {
            AccessToken = authResult.AccessToken!,
            RefreshToken = authResult.RefreshToken!,
            ExpiresAt = authResult.ExpiresAt!.Value,
            User = new UserResponse
            {
                Id = authResult.User!.Id,
                Email = authResult.User.Email,
                Name = authResult.User.Name,
                Slug = authResult.User.Slug,
                Timezone = authResult.User.Timezone
            }
        });
    }

    /// <summary>
    /// Get the current authenticated user's profile.
    /// </summary>
    /// <returns>User profile.</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var currentUserService = HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        if (!currentUserService.IsAuthenticated)
            return Unauthorized();

        var userRepository = HttpContext.RequestServices
            .GetRequiredService<ScheduleKit.Domain.Interfaces.IUserRepository>();

        var user = await userRepository.GetByIdAsync(currentUserService.UserId);

        if (user == null)
            return Unauthorized();

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
