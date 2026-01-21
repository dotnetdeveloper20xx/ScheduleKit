using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Availability;

/// <summary>
/// Command to delete an availability override.
/// </summary>
public record DeleteAvailabilityOverrideCommand : ICommand<bool>
{
    public Guid Id { get; init; }
    public Guid HostUserId { get; init; }
}

/// <summary>
/// Validator for DeleteAvailabilityOverrideCommand.
/// </summary>
public class DeleteAvailabilityOverrideCommandValidator
    : AbstractValidator<DeleteAvailabilityOverrideCommand>
{
    public DeleteAvailabilityOverrideCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Override ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");
    }
}

/// <summary>
/// Handler for DeleteAvailabilityOverrideCommand.
/// </summary>
public class DeleteAvailabilityOverrideCommandHandler
    : IRequestHandler<DeleteAvailabilityOverrideCommand, Result<bool>>
{
    private readonly IAvailabilityOverrideRepository _overrideRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAvailabilityOverrideCommandHandler(
        IAvailabilityOverrideRepository overrideRepository,
        IUnitOfWork unitOfWork)
    {
        _overrideRepository = overrideRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteAvailabilityOverrideCommand request,
        CancellationToken cancellationToken)
    {
        var availabilityOverride = await _overrideRepository.GetByIdAsync(
            request.Id, cancellationToken);

        if (availabilityOverride == null)
            return Result.Failure<bool>("Override not found.");

        // Verify ownership
        if (availabilityOverride.HostUserId != request.HostUserId)
            return Result.Failure<bool>("You do not have permission to delete this override.");

        _overrideRepository.Remove(availabilityOverride);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
