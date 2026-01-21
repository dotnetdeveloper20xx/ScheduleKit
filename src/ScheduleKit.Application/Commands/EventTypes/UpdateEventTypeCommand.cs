using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Mappings;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Application.Commands.EventTypes;

/// <summary>
/// Command to update an existing event type.
/// </summary>
public record UpdateEventTypeCommand : ICommand<EventTypeResponse>
{
    public Guid Id { get; init; }
    public Guid HostUserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DurationMinutes { get; init; }
    public int BufferBeforeMinutes { get; init; }
    public int BufferAfterMinutes { get; init; }
    public int MinimumNoticeMinutes { get; init; } = 60;
    public int BookingWindowDays { get; init; } = 60;
    public int? MaxBookingsPerDay { get; init; }
    public string LocationType { get; init; } = string.Empty;
    public string? LocationDetails { get; init; }
    public string? Color { get; init; }
}

/// <summary>
/// Validator for UpdateEventTypeCommand.
/// </summary>
public class UpdateEventTypeCommandValidator : AbstractValidator<UpdateEventTypeCommand>
{
    public UpdateEventTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(15, 480).WithMessage("Duration must be between 15 and 480 minutes.");

        RuleFor(x => x.BufferBeforeMinutes)
            .InclusiveBetween(0, 120).WithMessage("Buffer before must be between 0 and 120 minutes.");

        RuleFor(x => x.BufferAfterMinutes)
            .InclusiveBetween(0, 120).WithMessage("Buffer after must be between 0 and 120 minutes.");

        RuleFor(x => x.MinimumNoticeMinutes)
            .InclusiveBetween(0, 10080).WithMessage("Minimum notice must be between 0 and 10080 minutes (7 days).");

        RuleFor(x => x.BookingWindowDays)
            .InclusiveBetween(1, 365).WithMessage("Booking window must be between 1 and 365 days.");

        RuleFor(x => x.MaxBookingsPerDay)
            .GreaterThanOrEqualTo(1).When(x => x.MaxBookingsPerDay.HasValue)
            .WithMessage("Maximum bookings per day must be at least 1.");

        RuleFor(x => x.LocationType)
            .NotEmpty().WithMessage("Location type is required.")
            .Must(BeValidLocationType).WithMessage("Invalid location type.");
    }

    private static bool BeValidLocationType(string locationType)
    {
        return Enum.TryParse<LocationType>(locationType, true, out _);
    }
}

/// <summary>
/// Handler for UpdateEventTypeCommand.
/// </summary>
public class UpdateEventTypeCommandHandler : IRequestHandler<UpdateEventTypeCommand, Result<EventTypeResponse>>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEventTypeCommandHandler(
        IEventTypeRepository eventTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _eventTypeRepository = eventTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EventTypeResponse>> Handle(
        UpdateEventTypeCommand request,
        CancellationToken cancellationToken)
    {
        var eventType = await _eventTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (eventType is null)
        {
            return Result.Failure<EventTypeResponse>("Event type not found.");
        }

        // Verify ownership
        if (eventType.HostUserId != request.HostUserId)
        {
            return Result.Failure<EventTypeResponse>("You do not have permission to update this event type.");
        }

        // Parse location type
        if (!Enum.TryParse<LocationType>(request.LocationType, true, out var locationType))
        {
            return Result.Failure<EventTypeResponse>("Invalid location type.");
        }

        // Create meeting location
        MeetingLocation location = locationType switch
        {
            LocationType.Zoom => MeetingLocation.CreateZoom(request.LocationDetails),
            LocationType.GoogleMeet => MeetingLocation.CreateGoogleMeet(request.LocationDetails),
            LocationType.MicrosoftTeams => MeetingLocation.CreateMicrosoftTeams(request.LocationDetails),
            LocationType.Phone => MeetingLocation.CreatePhone(request.LocationDetails ?? "").Value,
            LocationType.InPerson => MeetingLocation.CreateInPerson(request.LocationDetails ?? "").Value,
            LocationType.Custom => MeetingLocation.CreateCustom(request.LocationDetails ?? "Custom", null).Value,
            _ => MeetingLocation.CreateZoom()
        };

        // Update event type
        var updateResult = eventType.UpdateDetails(
            request.Name,
            request.Description,
            request.DurationMinutes,
            request.BufferBeforeMinutes,
            request.BufferAfterMinutes,
            request.MinimumNoticeMinutes,
            request.BookingWindowDays,
            request.MaxBookingsPerDay,
            location,
            request.Color);

        if (updateResult.IsFailure)
        {
            return Result.Failure<EventTypeResponse>(updateResult.Error);
        }

        _eventTypeRepository.Update(eventType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(eventType.ToResponse());
    }
}
