using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Auth;

/// <summary>
/// Command to handle OAuth callback and login/register user.
/// </summary>
public record OAuthLoginCommand : ICommand<AuthResult>
{
    public string Provider { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string RedirectUri { get; init; } = string.Empty;
}

/// <summary>
/// Validator for OAuthLoginCommand.
/// </summary>
public class OAuthLoginCommandValidator : AbstractValidator<OAuthLoginCommand>
{
    public OAuthLoginCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Authorization code is required.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.");

        RuleFor(x => x.RedirectUri)
            .NotEmpty().WithMessage("Redirect URI is required.");
    }
}

/// <summary>
/// Handler for OAuthLoginCommand.
/// </summary>
public class OAuthLoginCommandHandler : IRequestHandler<OAuthLoginCommand, Result<AuthResult>>
{
    private readonly IOAuthService _oAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public OAuthLoginCommandHandler(
        IOAuthService oAuthService,
        IUserRepository userRepository,
        IAuthService authService,
        IUnitOfWork unitOfWork)
    {
        _oAuthService = oAuthService;
        _userRepository = userRepository;
        _authService = authService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResult>> Handle(
        OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        // Validate state parameter
        if (!_oAuthService.ValidateState(request.State))
        {
            return Result.Failure<AuthResult>("Invalid or expired state parameter.");
        }

        // Exchange code for user info
        var userInfo = await _oAuthService.ExchangeCodeAsync(
            request.Provider,
            request.Code,
            request.RedirectUri,
            cancellationToken);

        if (!userInfo.Success)
        {
            return Result.Failure<AuthResult>(userInfo.Error ?? "OAuth authentication failed.");
        }

        if (string.IsNullOrEmpty(userInfo.Email))
        {
            return Result.Failure<AuthResult>("Email not provided by OAuth provider.");
        }

        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(userInfo.Email, cancellationToken);

        User user;
        if (existingUser != null)
        {
            // Existing user - just login
            user = existingUser;
            user.RecordLogin();
        }
        else
        {
            // New user - create account
            var slug = GenerateSlug(userInfo.Name ?? userInfo.Email.Split('@')[0]);

            // Ensure unique slug
            var existingSlugUser = await _userRepository.GetBySlugAsync(slug, cancellationToken);
            if (existingSlugUser != null)
            {
                slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";
            }

            // Generate a unique password hash for OAuth users (they can't login with password)
            var oauthPasswordHash = $"oauth:{request.Provider}:{Guid.NewGuid()}";

            var userResult = User.Create(
                userInfo.Email,
                oauthPasswordHash,
                userInfo.Name ?? userInfo.Email.Split('@')[0],
                slug);

            if (userResult.IsFailure)
            {
                return Result.Failure<AuthResult>(userResult.Error);
            }

            user = userResult.Value;

            await _userRepository.AddAsync(user, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate JWT tokens
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

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "-");

        // Remove invalid characters
        slug = System.Text.RegularExpressions.Regex.Replace(slug, "[^a-z0-9-]", "");

        // Limit length
        if (slug.Length > 50)
        {
            slug = slug[..50];
        }

        return slug;
    }
}
