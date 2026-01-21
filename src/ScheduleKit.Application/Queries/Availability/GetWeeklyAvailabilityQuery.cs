using MediatR;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Queries.Availability;

/// <summary>
/// Query to get a host's weekly availability schedule.
/// </summary>
public record GetWeeklyAvailabilityQuery : IRequest<Result<WeeklyAvailabilityResponse>>
{
    public Guid HostUserId { get; init; }
}

/// <summary>
/// Handler for GetWeeklyAvailabilityQuery.
/// </summary>
public class GetWeeklyAvailabilityQueryHandler
    : IRequestHandler<GetWeeklyAvailabilityQuery, Result<WeeklyAvailabilityResponse>>
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetWeeklyAvailabilityQueryHandler(
        IAvailabilityRepository availabilityRepository,
        IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WeeklyAvailabilityResponse>> Handle(
        GetWeeklyAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        if (request.HostUserId == Guid.Empty)
            return Result.Failure<WeeklyAvailabilityResponse>("Host user ID is required.");

        // Get existing availability or create defaults
        var availabilities = await _availabilityRepository.GetByHostUserIdAsync(
            request.HostUserId, cancellationToken);

        if (availabilities.Count == 0)
        {
            // Create default availability
            availabilities = Domain.Entities.Availability.CreateDefaultWeek(request.HostUserId);
            await _availabilityRepository.AddRangeAsync(availabilities, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var response = new WeeklyAvailabilityResponse
        {
            HostUserId = request.HostUserId,
            Timezone = "UTC", // TODO: Get from user preferences
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
