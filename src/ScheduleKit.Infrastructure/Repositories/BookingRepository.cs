using Microsoft.EntityFrameworkCore;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Infrastructure.Data;

namespace ScheduleKit.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ScheduleKitDbContext _context;

    public BookingRepository(ScheduleKitDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Booking?> GetByIdWithResponsesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Responses)
            .Include(b => b.EventType)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .OrderByDescending(b => b.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetByHostUserIdAsync(
        Guid hostUserId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        BookingStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Bookings
            .Where(b => b.HostUserId == hostUserId);

        if (fromUtc.HasValue)
            query = query.Where(b => b.StartTimeUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(b => b.StartTimeUtc < toUtc.Value);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        return await query
            .Include(b => b.EventType)
            .OrderByDescending(b => b.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetByEventTypeIdAsync(
        Guid eventTypeId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Bookings
            .Where(b => b.EventTypeId == eventTypeId);

        if (fromUtc.HasValue)
            query = query.Where(b => b.StartTimeUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(b => b.StartTimeUtc < toUtc.Value);

        return await query
            .OrderByDescending(b => b.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetConflictingBookingsAsync(
        Guid hostUserId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Bookings
            .Where(b => b.HostUserId == hostUserId)
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.StartTimeUtc < endTimeUtc && b.EndTimeUtc > startTimeUtc);

        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Booking?> GetByRescheduleTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.EventType)
            .FirstOrDefaultAsync(b => b.RescheduleToken == token, cancellationToken);
    }

    public async Task<int> GetBookingCountForDateAsync(
        Guid hostUserId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        return await _context.Bookings
            .CountAsync(b =>
                b.HostUserId == hostUserId &&
                b.Status == BookingStatus.Confirmed &&
                b.StartTimeUtc >= startOfDay &&
                b.StartTimeUtc < endOfDay,
                cancellationToken);
    }

    public async Task AddAsync(Booking entity, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(entity, cancellationToken);
    }

    public void Update(Booking entity)
    {
        _context.Bookings.Update(entity);
    }

    public void Remove(Booking entity)
    {
        _context.Bookings.Remove(entity);
    }
}
