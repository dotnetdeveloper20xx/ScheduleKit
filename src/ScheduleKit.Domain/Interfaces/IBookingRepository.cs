using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Domain.Interfaces;

/// <summary>
/// Repository interface for Booking aggregate.
/// </summary>
public interface IBookingRepository : IRepository<Booking>
{
    Task<List<Booking>> GetByHostUserIdAsync(
        Guid hostUserId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        BookingStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<List<Booking>> GetByEventTypeIdAsync(
        Guid eventTypeId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default);

    Task<List<Booking>> GetConflictingBookingsAsync(
        Guid hostUserId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default);

    Task<Booking?> GetByIdWithResponsesAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Booking?> GetByRescheduleTokenAsync(string token, CancellationToken cancellationToken = default);

    Task<int> GetBookingCountForDateAsync(
        Guid hostUserId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<List<Booking>> GetBookingsNeedingReminderAsync(
        int reminderHoursBefore,
        int batchSize,
        CancellationToken cancellationToken = default);
}
