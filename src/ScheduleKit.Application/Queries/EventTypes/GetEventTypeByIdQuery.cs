using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Mappings;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Queries.EventTypes;

/// <summary>
/// Query to get a single event type by ID.
/// </summary>
public record GetEventTypeByIdQuery : IQuery<EventTypeResponse>
{
    public Guid Id { get; init; }
    public Guid HostUserId { get; init; }
}

/// <summary>
/// Validator for GetEventTypeByIdQuery.
/// </summary>
public class GetEventTypeByIdQueryValidator : AbstractValidator<GetEventTypeByIdQuery>
{
    public GetEventTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");
    }
}

/// <summary>
/// Handler for GetEventTypeByIdQuery.
/// </summary>
public class GetEventTypeByIdQueryHandler : IRequestHandler<GetEventTypeByIdQuery, Result<EventTypeResponse>>
{
    private readonly IEventTypeRepository _eventTypeRepository;

    public GetEventTypeByIdQueryHandler(IEventTypeRepository eventTypeRepository)
    {
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<Result<EventTypeResponse>> Handle(
        GetEventTypeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var eventType = await _eventTypeRepository.GetByIdWithQuestionsAsync(request.Id, cancellationToken);

        if (eventType is null)
        {
            return Result.Failure<EventTypeResponse>("Event type not found.");
        }

        // Verify ownership
        if (eventType.HostUserId != request.HostUserId)
        {
            return Result.Failure<EventTypeResponse>("You do not have permission to view this event type.");
        }

        return Result.Success(eventType.ToResponse());
    }
}
