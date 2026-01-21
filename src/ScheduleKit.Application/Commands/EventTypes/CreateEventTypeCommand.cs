using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Mappings;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Application.Commands.EventTypes;

/// <summary>
/// Command to create a new event type.
/// </summary>
public record CreateEventTypeCommand : ICommand<EventTypeResponse>
{
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
/// Validator for CreateEventTypeCommand.
/// </summary>
public class CreateEventTypeCommandValidator : AbstractValidator<CreateEventTypeCommand>
{
    public CreateEventTypeCommandValidator()
    {
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
/// Handler for CreateEventTypeCommand.
/// </summary>
public class CreateEventTypeCommandHandler : IRequestHandler<CreateEventTypeCommand, Result<EventTypeResponse>>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventTypeCommandHandler(
        IEventTypeRepository eventTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _eventTypeRepository = eventTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EventTypeResponse>> Handle(
        CreateEventTypeCommand request,
        CancellationToken cancellationToken)
    {
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

        // Create event type
        var eventTypeResult = EventType.Create(
            request.HostUserId,
            request.Name,
            request.DurationMinutes,
            location,
            request.Description,
            request.BufferBeforeMinutes,
            request.BufferAfterMinutes,
            request.MinimumNoticeMinutes,
            request.BookingWindowDays,
            request.MaxBookingsPerDay,
            request.Color);

        if (eventTypeResult.IsFailure)
        {
            return Result.Failure<EventTypeResponse>(eventTypeResult.Error);
        }

        var eventType = eventTypeResult.Value;

        // Check for slug uniqueness and make unique if needed
        var baseSlug = eventType.Slug.Value;
        var finalSlug = baseSlug;
        var counter = 1;

        while (await _eventTypeRepository.SlugExistsAsync(request.HostUserId, finalSlug, null, cancellationToken))
        {
            finalSlug = $"{baseSlug}-{counter}";
            counter++;

            if (counter > 100)
            {
                return Result.Failure<EventTypeResponse>("Unable to generate unique slug.");
            }
        }

        if (finalSlug != baseSlug)
        {
            eventType.UpdateSlug(finalSlug);
        }

        await _eventTypeRepository.AddAsync(eventType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(eventType.ToResponse());
    }
}
