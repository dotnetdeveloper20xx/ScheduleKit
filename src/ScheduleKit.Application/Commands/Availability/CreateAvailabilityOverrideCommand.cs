using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Availability;

/// <summary>
/// Command to create an availability override (block a day/time or add extra availability).
/// </summary>
public record CreateAvailabilityOverrideCommand : ICommand<AvailabilityOverrideResponse>
{
    public Guid HostUserId { get; init; }
    public string Date { get; init; } = string.Empty;
    public string? StartTime { get; init; }
    public string? EndTime { get; init; }
    public bool IsBlocked { get; init; } = true;
    public string? Reason { get; init; }
}

/// <summary>
/// Validator for CreateAvailabilityOverrideCommand.
/// </summary>
public class CreateAvailabilityOverrideCommandValidator
    : AbstractValidator<CreateAvailabilityOverrideCommand>
{
    public CreateAvailabilityOverrideCommandValidator()
    {
        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Date must be in yyyy-MM-dd format.");

        RuleFor(x => x.StartTime)
            .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")
            .When(x => !string.IsNullOrEmpty(x.StartTime))
            .WithMessage("Start time must be in HH:mm format.");

        RuleFor(x => x.EndTime)
            .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")
            .When(x => !string.IsNullOrEmpty(x.EndTime))
            .WithMessage("End time must be in HH:mm format.");

        RuleFor(x => x.Reason)
            .MaximumLength(200).WithMessage("Reason must not exceed 200 characters.");

        // If adding extra availability (not blocked), times are required
        RuleFor(x => x)
            .Must(x => x.IsBlocked || (!string.IsNullOrEmpty(x.StartTime) && !string.IsNullOrEmpty(x.EndTime)))
            .WithMessage("Start time and end time are required when adding extra availability.");
    }
}

/// <summary>
/// Handler for CreateAvailabilityOverrideCommand.
/// </summary>
public class CreateAvailabilityOverrideCommandHandler
    : IRequestHandler<CreateAvailabilityOverrideCommand, Result<AvailabilityOverrideResponse>>
{
    private readonly IAvailabilityOverrideRepository _overrideRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAvailabilityOverrideCommandHandler(
        IAvailabilityOverrideRepository overrideRepository,
        IUnitOfWork unitOfWork)
    {
        _overrideRepository = overrideRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AvailabilityOverrideResponse>> Handle(
        CreateAvailabilityOverrideCommand request,
        CancellationToken cancellationToken)
    {
        // Parse date
        if (!DateOnly.TryParse(request.Date, out var date))
            return Result.Failure<AvailabilityOverrideResponse>("Invalid date format.");

        // Check for existing override on this date
        var existing = await _overrideRepository.GetByDateAsync(
            request.HostUserId, date, cancellationToken);

        if (existing != null)
            return Result.Failure<AvailabilityOverrideResponse>(
                "An override already exists for this date. Delete it first to create a new one.");

        Result<AvailabilityOverride> overrideResult;

        if (request.IsBlocked)
        {
            // Creating a blocked day/time
            if (string.IsNullOrEmpty(request.StartTime) || string.IsNullOrEmpty(request.EndTime))
            {
                // Block entire day
                overrideResult = AvailabilityOverride.CreateBlockedDay(
                    request.HostUserId, date, request.Reason);
            }
            else
            {
                // Block specific time range
                if (!TimeOnly.TryParse(request.StartTime, out var startTime))
                    return Result.Failure<AvailabilityOverrideResponse>("Invalid start time format.");

                if (!TimeOnly.TryParse(request.EndTime, out var endTime))
                    return Result.Failure<AvailabilityOverrideResponse>("Invalid end time format.");

                overrideResult = AvailabilityOverride.CreateBlockedTimeRange(
                    request.HostUserId, date, startTime, endTime, request.Reason);
            }
        }
        else
        {
            // Adding extra availability
            if (!TimeOnly.TryParse(request.StartTime, out var startTime))
                return Result.Failure<AvailabilityOverrideResponse>("Invalid start time format.");

            if (!TimeOnly.TryParse(request.EndTime, out var endTime))
                return Result.Failure<AvailabilityOverrideResponse>("Invalid end time format.");

            overrideResult = AvailabilityOverride.CreateExtraAvailability(
                request.HostUserId, date, startTime, endTime, request.Reason);
        }

        if (overrideResult.IsFailure)
            return Result.Failure<AvailabilityOverrideResponse>(overrideResult.Error);

        var availabilityOverride = overrideResult.Value;
        await _overrideRepository.AddAsync(availabilityOverride, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AvailabilityOverrideResponse
        {
            Id = availabilityOverride.Id,
            HostUserId = availabilityOverride.HostUserId,
            Date = availabilityOverride.Date.ToString("yyyy-MM-dd"),
            StartTime = availabilityOverride.StartTime?.ToString("HH:mm"),
            EndTime = availabilityOverride.EndTime?.ToString("HH:mm"),
            IsBlocked = availabilityOverride.IsBlocked,
            IsFullDayBlock = availabilityOverride.IsFullDayBlock,
            Reason = availabilityOverride.Reason,
            CreatedAtUtc = availabilityOverride.CreatedAtUtc
        });
    }
}
