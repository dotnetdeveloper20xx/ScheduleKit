using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Auth;

/// <summary>
/// Command to login a user.
/// </summary>
public record LoginCommand : ICommand<AuthResult>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Validator for LoginCommand.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

/// <summary>
/// Handler for LoginCommand.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IAuthService authService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _authService = authService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResult>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
            return Result.Failure<AuthResult>("Invalid email or password.");

        if (!user.IsActive)
            return Result.Failure<AuthResult>("This account has been deactivated.");

        // Verify password
        if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
            return Result.Failure<AuthResult>("Invalid email or password.");

        // Record login
        user.RecordLogin();
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
