using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Domain.Interfaces;

/// <summary>
/// Repository interface for EventType aggregate.
/// </summary>
public interface IEventTypeRepository : IRepository<EventType>
{
    Task<EventType?> GetBySlugAsync(Guid hostUserId, string slug, CancellationToken cancellationToken = default);
    Task<List<EventType>> GetByHostUserIdAsync(Guid hostUserId, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(Guid hostUserId, string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<EventType?> GetByIdWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default);
}
