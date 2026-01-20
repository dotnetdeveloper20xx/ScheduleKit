namespace ScheduleKit.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides common functionality like domain events and equality.
/// </summary>
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; protected set; }
    public DateTime CreatedAtUtc { get; protected set; }
    public DateTime? UpdatedAtUtc { get; protected set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void SetCreatedAt()
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    protected void SetUpdatedAt()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

/// <summary>
/// Marker interface for aggregate roots.
/// Aggregates are the only entities that can be directly loaded from repositories.
/// </summary>
public interface IAggregateRoot
{
}

/// <summary>
/// Marker interface for domain events.
/// Domain events are raised when something significant happens in the domain.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}
