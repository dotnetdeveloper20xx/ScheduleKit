using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Users;

/// <summary>
/// Command to update a user's profile.
/// </summary>
public record UpdateProfileCommand : ICommand
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Slug { get; init; }
}

/// <summary>
/// Validator for UpdateProfileCommand.
/// </summary>
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Slug)
            .Matches(@"^[a-z0-9-]+$")
            .When(x => !string.IsNullOrEmpty(x.Slug))
            .WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.")
            .MinimumLength(3).When(x => !string.IsNullOrEmpty(x.Slug))
            .WithMessage("Slug must be at least 3 characters.")
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Slug))
            .WithMessage("Slug must not exceed 50 characters.");
    }
}

/// <summary>
/// Handler for UpdateProfileCommand.
/// </summary>
public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result.Failure("User not found.");

        // Check if slug is taken by another user
        if (!string.IsNullOrEmpty(request.Slug) &&
            await _userRepository.SlugExistsAsync(request.Slug, request.UserId, cancellationToken))
        {
            return Result.Failure("This username is already taken.");
        }

        var result = user.UpdateProfile(request.Name, request.Slug);

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
