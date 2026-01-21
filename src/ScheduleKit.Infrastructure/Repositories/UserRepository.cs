using Microsoft.EntityFrameworkCore;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Infrastructure.Data;

namespace ScheduleKit.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User aggregate.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ScheduleKitDbContext _context;

    public UserRepository(ScheduleKitDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Slug == normalizedSlug, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _context.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.ToLowerInvariant();
        var query = _context.Users.Where(u => u.Slug == normalizedSlug);

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(entity, cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public void Update(User entity)
    {
        _context.Users.Update(entity);
    }

    public void Remove(User entity)
    {
        _context.Users.Remove(entity);
    }
}
