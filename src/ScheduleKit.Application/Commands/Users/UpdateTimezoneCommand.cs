using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Users;

/// <summary>
/// Command to update a user's timezone.
/// </summary>
public record UpdateTimezoneCommand : ICommand
{
    public Guid UserId { get; init; }
    public string Timezone { get; init; } = string.Empty;
}

/// <summary>
/// Validator for UpdateTimezoneCommand.
/// </summary>
public class UpdateTimezoneCommandValidator : AbstractValidator<UpdateTimezoneCommand>
{
    public UpdateTimezoneCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Timezone)
            .NotEmpty().WithMessage("Timezone is required.");
    }
}

/// <summary>
/// Handler for UpdateTimezoneCommand.
/// </summary>
public class UpdateTimezoneCommandHandler : IRequestHandler<UpdateTimezoneCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTimezoneCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateTimezoneCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result.Failure("User not found.");

        var result = user.UpdateTimezone(request.Timezone);

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
