using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Auth;

/// <summary>
/// Command to register a new user.
/// </summary>
public record RegisterCommand : ICommand<AuthResult>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Slug { get; init; }
}

/// <summary>
/// Validator for RegisterCommand.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Slug)
            .Matches(@"^[a-z0-9-]+$")
            .When(x => !string.IsNullOrEmpty(x.Slug))
            .WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");
    }
}

/// <summary>
/// Handler for RegisterCommand.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IAuthService authService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _authService = authService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResult>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // Check if email is already registered
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            return Result.Failure<AuthResult>("An account with this email already exists.");

        // Check if slug is taken (if provided)
        if (!string.IsNullOrEmpty(request.Slug) &&
            await _userRepository.SlugExistsAsync(request.Slug, cancellationToken: cancellationToken))
            return Result.Failure<AuthResult>("This username is already taken.");

        // Hash password
        var passwordHash = _authService.HashPassword(request.Password);

        // Create user
        var userResult = User.Create(
            request.Email,
            passwordHash,
            request.Name,
            request.Slug);

        if (userResult.IsFailure)
            return Result.Failure<AuthResult>(userResult.Error);

        var user = userResult.Value;

        // Save user
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _authService.GenerateToken(user.Id, user.Email, user.Name);
        var refreshToken = _authService.GenerateRefreshToken();

        return Result.Success(AuthResult.Succeeded(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserInfo
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Slug = user.Slug,
                Timezone = user.Timezone
            }));
    }
}
