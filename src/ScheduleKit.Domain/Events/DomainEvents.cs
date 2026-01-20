using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.Events;

/// <summary>
/// Base record for domain events with timestamp.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when a new event type is created.
/// </summary>
public sealed record EventTypeCreatedEvent(Guid EventTypeId, Guid HostUserId, string Name) : DomainEventBase;

/// <summary>
/// Raised when an event type is updated.
/// </summary>
public sealed record EventTypeUpdatedEvent(Guid EventTypeId, string Name) : DomainEventBase;

/// <summary>
/// Raised when an event type is deleted.
/// </summary>
public sealed record EventTypeDeletedEvent(Guid EventTypeId, Guid HostUserId) : DomainEventBase;

/// <summary>
/// Raised when a new booking is created.
/// </summary>
public sealed record BookingCreatedEvent(
    Guid BookingId,
    Guid EventTypeId,
    Guid HostUserId,
    string GuestEmail,
    DateTime StartTimeUtc) : DomainEventBase;

/// <summary>
/// Who cancelled the booking.
/// </summary>
public enum CancelledBy
{
    Host,
    Guest,
    System
}

/// <summary>
/// Raised when a booking is cancelled.
/// </summary>
public sealed record BookingCancelledEvent(
    Guid BookingId,
    Guid HostUserId,
    string GuestEmail,
    DateTime StartTimeUtc,
    CancelledBy CancelledBy,
    string? Reason) : DomainEventBase;

/// <summary>
/// Raised when a booking is rescheduled.
/// </summary>
public sealed record BookingRescheduledEvent(
    Guid BookingId,
    DateTime OldStartTimeUtc,
    DateTime NewStartTimeUtc) : DomainEventBase;

/// <summary>
/// Raised when availability is updated.
/// </summary>
public sealed record AvailabilityUpdatedEvent(Guid HostUserId) : DomainEventBase;
