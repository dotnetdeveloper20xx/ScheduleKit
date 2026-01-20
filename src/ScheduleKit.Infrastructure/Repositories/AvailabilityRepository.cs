using Microsoft.EntityFrameworkCore;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Infrastructure.Data;

namespace ScheduleKit.Infrastructure.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly ScheduleKitDbContext _context;

    public AvailabilityRepository(ScheduleKitDbContext context)
    {
        _context = context;
    }

    public async Task<List<Availability>> GetByHostUserIdAsync(Guid hostUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Availabilities
            .Where(a => a.HostUserId == hostUserId)
            .OrderBy(a => a.DayOfWeek)
            .ToListAsync(cancellationToken);
    }

    public async Task<Availability?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Availabilities
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Availability> availabilities, CancellationToken cancellationToken = default)
    {
        await _context.Availabilities.AddRangeAsync(availabilities, cancellationToken);
    }

    public void UpdateRange(IEnumerable<Availability> availabilities)
    {
        _context.Availabilities.UpdateRange(availabilities);
    }

    public async Task<bool> ExistsForHostAsync(Guid hostUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Availabilities
            .AnyAsync(a => a.HostUserId == hostUserId, cancellationToken);
    }
}

public class AvailabilityOverrideRepository : IAvailabilityOverrideRepository
{
    private readonly ScheduleKitDbContext _context;

    public AvailabilityOverrideRepository(ScheduleKitDbContext context)
    {
        _context = context;
    }

    public async Task<List<AvailabilityOverride>> GetByHostUserIdAsync(
        Guid hostUserId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AvailabilityOverrides
            .Where(a => a.HostUserId == hostUserId);

        if (fromDate.HasValue)
            query = query.Where(a => a.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.Date <= toDate.Value);

        return await query
            .OrderBy(a => a.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<AvailabilityOverride?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AvailabilityOverrides
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<AvailabilityOverride?> GetByDateAsync(
        Guid hostUserId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _context.AvailabilityOverrides
            .FirstOrDefaultAsync(a => a.HostUserId == hostUserId && a.Date == date, cancellationToken);
    }

    public async Task AddAsync(AvailabilityOverride availabilityOverride, CancellationToken cancellationToken = default)
    {
        await _context.AvailabilityOverrides.AddAsync(availabilityOverride, cancellationToken);
    }

    public void Remove(AvailabilityOverride availabilityOverride)
    {
        _context.AvailabilityOverrides.Remove(availabilityOverride);
    }
}
