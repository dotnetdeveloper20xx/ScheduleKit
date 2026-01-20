using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Domain.Interfaces;

/// <summary>
/// Repository interface for Availability.
/// Note: Availability is not an aggregate root, but managed through this repository.
/// </summary>
public interface IAvailabilityRepository
{
    Task<List<Availability>> GetByHostUserIdAsync(Guid hostUserId, CancellationToken cancellationToken = default);
    Task<Availability?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Availability> availabilities, CancellationToken cancellationToken = default);
    void UpdateRange(IEnumerable<Availability> availabilities);
    Task<bool> ExistsForHostAsync(Guid hostUserId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for AvailabilityOverride.
/// </summary>
public interface IAvailabilityOverrideRepository
{
    Task<List<AvailabilityOverride>> GetByHostUserIdAsync(
        Guid hostUserId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);

    Task<AvailabilityOverride?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AvailabilityOverride?> GetByDateAsync(
        Guid hostUserId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task AddAsync(AvailabilityOverride availabilityOverride, CancellationToken cancellationToken = default);
    void Remove(AvailabilityOverride availabilityOverride);
}
