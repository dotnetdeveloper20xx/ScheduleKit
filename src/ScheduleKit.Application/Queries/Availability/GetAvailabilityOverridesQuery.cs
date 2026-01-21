using MediatR;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Queries.Availability;

/// <summary>
/// Query to get a host's availability overrides.
/// </summary>
public record GetAvailabilityOverridesQuery : IRequest<Result<List<AvailabilityOverrideResponse>>>
{
    public Guid HostUserId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
}

/// <summary>
/// Handler for GetAvailabilityOverridesQuery.
/// </summary>
public class GetAvailabilityOverridesQueryHandler
    : IRequestHandler<GetAvailabilityOverridesQuery, Result<List<AvailabilityOverrideResponse>>>
{
    private readonly IAvailabilityOverrideRepository _overrideRepository;

    public GetAvailabilityOverridesQueryHandler(IAvailabilityOverrideRepository overrideRepository)
    {
        _overrideRepository = overrideRepository;
    }

    public async Task<Result<List<AvailabilityOverrideResponse>>> Handle(
        GetAvailabilityOverridesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.HostUserId == Guid.Empty)
            return Result.Failure<List<AvailabilityOverrideResponse>>("Host user ID is required.");

        var overrides = await _overrideRepository.GetByHostUserIdAsync(
            request.HostUserId,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var response = overrides
            .Select(o => new AvailabilityOverrideResponse
            {
                Id = o.Id,
                HostUserId = o.HostUserId,
                Date = o.Date.ToString("yyyy-MM-dd"),
                StartTime = o.StartTime?.ToString("HH:mm"),
                EndTime = o.EndTime?.ToString("HH:mm"),
                IsBlocked = o.IsBlocked,
                IsFullDayBlock = o.IsFullDayBlock,
                Reason = o.Reason,
                CreatedAtUtc = o.CreatedAtUtc
            })
            .ToList();

        return Result.Success(response);
    }
}
