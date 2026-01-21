using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Domain.Interfaces;

/// <summary>
/// Repository interface for User aggregate.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user by email address.
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by slug.
    /// </summary>
    Task<User?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email is already registered.
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a slug is already taken.
    /// </summary>
    Task<bool> SlugExistsAsync(string slug, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
}
