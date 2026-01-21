using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Availability;

/// <summary>
/// Command to update a host's weekly availability schedule.
/// </summary>
public record UpdateWeeklyAvailabilityCommand : ICommand<WeeklyAvailabilityResponse>
{
    public Guid HostUserId { get; init; }
    public List<DayAvailabilityUpdate> Days { get; init; } = new();
}

/// <summary>
/// Input for updating a single day's availability.
/// </summary>
public record DayAvailabilityUpdate
{
    public DayOfWeek DayOfWeek { get; init; }
    public string StartTime { get; init; } = string.Empty;
    public string EndTime { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
}

/// <summary>
/// Validator for UpdateWeeklyAvailabilityCommand.
/// </summary>
public class UpdateWeeklyAvailabilityCommandValidator
    : AbstractValidator<UpdateWeeklyAvailabilityCommand>
{
    public UpdateWeeklyAvailabilityCommandValidator()
    {
        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");

        RuleFor(x => x.Days)
            .NotEmpty().WithMessage("At least one day must be provided.")
            .Must(days => days.Count <= 7).WithMessage("Cannot have more than 7 days.");

        RuleForEach(x => x.Days).ChildRules(day =>
        {
            day.RuleFor(d => d.StartTime)
                .NotEmpty().WithMessage("Start time is required.")
                .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")
                .WithMessage("Start time must be in HH:mm format.");

            day.RuleFor(d => d.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")
                .WithMessage("End time must be in HH:mm format.");
        });
    }
}

/// <summary>
/// Handler for UpdateWeeklyAvailabilityCommand.
/// </summary>
public class UpdateWeeklyAvailabilityCommandHandler
    : IRequestHandler<UpdateWeeklyAvailabilityCommand, Result<WeeklyAvailabilityResponse>>
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWeeklyAvailabilityCommandHandler(
        IAvailabilityRepository availabilityRepository,
        IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WeeklyAvailabilityResponse>> Handle(
        UpdateWeeklyAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        // Get existing availability
        var availabilities = await _availabilityRepository.GetByHostUserIdAsync(
            request.HostUserId, cancellationToken);

        // If no existing availability, create defaults first
        if (availabilities.Count == 0)
        {
            availabilities = Domain.Entities.Availability.CreateDefaultWeek(request.HostUserId);
            await _availabilityRepository.AddRangeAsync(availabilities, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Update each day
        foreach (var dayUpdate in request.Days)
        {
            var availability = availabilities.FirstOrDefault(a => a.DayOfWeek == dayUpdate.DayOfWeek);
            if (availability == null)
                continue;

            if (!TimeOnly.TryParse(dayUpdate.StartTime, out var startTime))
                return Result.Failure<WeeklyAvailabilityResponse>(
                    $"Invalid start time format for {dayUpdate.DayOfWeek}.");

            if (!TimeOnly.TryParse(dayUpdate.EndTime, out var endTime))
                return Result.Failure<WeeklyAvailabilityResponse>(
                    $"Invalid end time format for {dayUpdate.DayOfWeek}.");

            var updateResult = availability.Update(startTime, endTime, dayUpdate.IsEnabled);
            if (updateResult.IsFailure)
                return Result.Failure<WeeklyAvailabilityResponse>(
                    $"Failed to update {dayUpdate.DayOfWeek}: {updateResult.Error}");
        }

        _availabilityRepository.UpdateRange(availabilities);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return updated availability
        var response = new WeeklyAvailabilityResponse
        {
            HostUserId = request.HostUserId,
            Timezone = "UTC",
            Days = availabilities
                .OrderBy(a => a.DayOfWeek)
                .Select(a => new DayAvailabilityResponse
                {
                    Id = a.Id,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime.ToString("HH:mm"),
                    EndTime = a.EndTime.ToString("HH:mm"),
                    IsEnabled = a.IsEnabled
                })
                .ToList()
        };

        return Result.Success(response);
    }
}
