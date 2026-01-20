using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Mappings;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Queries.EventTypes;

/// <summary>
/// Query to get all event types for a host.
/// </summary>
public record GetEventTypesQuery : IQuery<List<EventTypeResponse>>
{
    public Guid HostUserId { get; init; }
}

/// <summary>
/// Validator for GetEventTypesQuery.
/// </summary>
public class GetEventTypesQueryValidator : AbstractValidator<GetEventTypesQuery>
{
    public GetEventTypesQueryValidator()
    {
        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");
    }
}

/// <summary>
/// Handler for GetEventTypesQuery.
/// </summary>
public class GetEventTypesQueryHandler : IRequestHandler<GetEventTypesQuery, Result<List<EventTypeResponse>>>
{
    private readonly IEventTypeRepository _eventTypeRepository;

    public GetEventTypesQueryHandler(IEventTypeRepository eventTypeRepository)
    {
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<Result<List<EventTypeResponse>>> Handle(
        GetEventTypesQuery request,
        CancellationToken cancellationToken)
    {
        var eventTypes = await _eventTypeRepository.GetByHostUserIdAsync(request.HostUserId, cancellationToken);
        return Result.Success(eventTypes.ToResponseList());
    }
}
