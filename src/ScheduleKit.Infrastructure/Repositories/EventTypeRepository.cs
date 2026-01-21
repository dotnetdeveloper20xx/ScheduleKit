using Microsoft.EntityFrameworkCore;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Infrastructure.Data;

namespace ScheduleKit.Infrastructure.Repositories;

public class EventTypeRepository : IEventTypeRepository
{
    private readonly ScheduleKitDbContext _context;

    public EventTypeRepository(ScheduleKitDbContext context)
    {
        _context = context;
    }

    public async Task<EventType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EventTypes
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<EventType?> GetByIdWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EventTypes
            .Include(e => e.Questions.OrderBy(q => q.DisplayOrder))
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<EventType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EventTypes
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventType?> GetBySlugAsync(Guid hostUserId, string slug, CancellationToken cancellationToken = default)
    {
        return await _context.EventTypes
            .Include(e => e.Questions.OrderBy(q => q.DisplayOrder))
            .FirstOrDefaultAsync(e => e.HostUserId == hostUserId && e.Slug.Value == slug, cancellationToken);
    }

    public async Task<EventType?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.EventTypes
            .Include(e => e.Questions.OrderBy(q => q.DisplayOrder))
            .FirstOrDefaultAsync(e => e.Slug.Value == slug, cancellationToken);
    }

    public async Task<List<EventType>> GetByHostUserIdAsync(Guid hostUserId, CancellationToken cancellationToken = default)
    {
        return await _context.EventTypes
            .Where(e => e.HostUserId == hostUserId)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(Guid hostUserId, string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.EventTypes
            .Where(e => e.HostUserId == hostUserId && e.Slug.Value == slug);

        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(EventType entity, CancellationToken cancellationToken = default)
    {
        await _context.EventTypes.AddAsync(entity, cancellationToken);
    }

    public void Update(EventType entity)
    {
        _context.EventTypes.Update(entity);
    }

    public void Remove(EventType entity)
    {
        _context.EventTypes.Remove(entity);
    }
}
