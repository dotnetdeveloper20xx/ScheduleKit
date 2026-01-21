using MediatR;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Mappings;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Queries.EventTypes;

/// <summary>
/// Query to get an event type by its slug.
/// Used by the public booking page.
/// </summary>
public record GetEventTypeBySlugQuery : IRequest<Result<EventTypeResponse>>
{
    public string Slug { get; init; } = string.Empty;
}

/// <summary>
/// Handler for GetEventTypeBySlugQuery.
/// </summary>
public class GetEventTypeBySlugQueryHandler
    : IRequestHandler<GetEventTypeBySlugQuery, Result<EventTypeResponse>>
{
    private readonly IEventTypeRepository _eventTypeRepository;

    public GetEventTypeBySlugQueryHandler(IEventTypeRepository eventTypeRepository)
    {
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<Result<EventTypeResponse>> Handle(
        GetEventTypeBySlugQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Slug))
            return Result.Failure<EventTypeResponse>("Slug is required.");

        var eventType = await _eventTypeRepository.GetBySlugAsync(request.Slug, cancellationToken);

        if (eventType == null)
            return Result.Failure<EventTypeResponse>("Event type not found.");

        return Result.Success(eventType.ToResponse());
    }
}
