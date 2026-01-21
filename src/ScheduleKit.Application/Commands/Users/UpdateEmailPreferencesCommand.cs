using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Users;

/// <summary>
/// Command to update a user's email notification preferences.
/// </summary>
public record UpdateEmailPreferencesCommand : ICommand
{
    public Guid UserId { get; init; }
    public bool EmailNotificationsEnabled { get; init; }
    public bool ReminderEmailsEnabled { get; init; }
    public int ReminderHoursBefore { get; init; }
}

/// <summary>
/// Validator for UpdateEmailPreferencesCommand.
/// </summary>
public class UpdateEmailPreferencesCommandValidator : AbstractValidator<UpdateEmailPreferencesCommand>
{
    public UpdateEmailPreferencesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ReminderHoursBefore)
            .InclusiveBetween(1, 168)
            .WithMessage("Reminder hours must be between 1 and 168 (1 week).");
    }
}

/// <summary>
/// Handler for UpdateEmailPreferencesCommand.
/// </summary>
public class UpdateEmailPreferencesCommandHandler : IRequestHandler<UpdateEmailPreferencesCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmailPreferencesCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateEmailPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result.Failure("User not found.");

        var result = user.UpdateEmailPreferences(
            request.EmailNotificationsEnabled,
            request.ReminderEmailsEnabled,
            request.ReminderHoursBefore);

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
