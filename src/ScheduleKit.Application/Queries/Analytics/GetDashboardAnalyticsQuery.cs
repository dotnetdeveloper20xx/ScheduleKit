using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Queries.Analytics;

/// <summary>
/// Response DTO for dashboard analytics.
/// </summary>
public record DashboardAnalyticsResponse
{
    public int TotalBookings { get; init; }
    public int ConfirmedBookings { get; init; }
    public int CancelledBookings { get; init; }
    public int CompletedBookings { get; init; }
    public int NoShowBookings { get; init; }
    public double CompletionRate { get; init; }
    public double CancellationRate { get; init; }
    public int UpcomingBookings { get; init; }
    public int TodaysBookings { get; init; }
    public int ThisWeeksBookings { get; init; }
    public int ThisMonthsBookings { get; init; }
    public List<DailyBookingCount> BookingsLast30Days { get; init; } = new();
    public List<HourlyBookingCount> PopularHours { get; init; } = new();
    public List<EventTypeBookingCount> TopEventTypes { get; init; } = new();
}

public record DailyBookingCount
{
    public string Date { get; init; } = string.Empty;
    public int Count { get; init; }
}

public record HourlyBookingCount
{
    public int Hour { get; init; }
    public int Count { get; init; }
}

public record EventTypeBookingCount
{
    public string EventTypeName { get; init; } = string.Empty;
    public int Count { get; init; }
}

/// <summary>
/// Query to get dashboard analytics.
/// </summary>
public record GetDashboardAnalyticsQuery : IQuery<DashboardAnalyticsResponse>
{
    public Guid HostUserId { get; init; }
}

/// <summary>
/// Validator for GetDashboardAnalyticsQuery.
/// </summary>
public class GetDashboardAnalyticsQueryValidator : AbstractValidator<GetDashboardAnalyticsQuery>
{
    public GetDashboardAnalyticsQueryValidator()
    {
        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");
    }
}

/// <summary>
/// Handler for GetDashboardAnalyticsQuery.
/// </summary>
public class GetDashboardAnalyticsQueryHandler
    : IRequestHandler<GetDashboardAnalyticsQuery, Result<DashboardAnalyticsResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventTypeRepository _eventTypeRepository;

    public GetDashboardAnalyticsQueryHandler(
        IBookingRepository bookingRepository,
        IEventTypeRepository eventTypeRepository)
    {
        _bookingRepository = bookingRepository;
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<Result<DashboardAnalyticsResponse>> Handle(
        GetDashboardAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var thirtyDaysAgo = today.AddDays(-30);

        // Get all bookings for the host
        var allBookings = await _bookingRepository.GetByHostUserIdAsync(
            request.HostUserId,
            cancellationToken: cancellationToken);

        // Get event types for names
        var eventTypes = await _eventTypeRepository.GetByHostUserIdAsync(
            request.HostUserId, cancellationToken);

        var eventTypeNames = eventTypes.ToDictionary(e => e.Id, e => e.Name);

        // Calculate totals by status
        var confirmed = allBookings.Count(b => b.Status == BookingStatus.Confirmed);
        var cancelled = allBookings.Count(b => b.Status == BookingStatus.Cancelled);
        var completed = allBookings.Count(b => b.Status == BookingStatus.Completed);
        var noShow = allBookings.Count(b => b.Status == BookingStatus.NoShow);
        var total = allBookings.Count;

        // Calculate rates
        var nonCancelledTotal = confirmed + completed + noShow;
        var completionRate = nonCancelledTotal > 0 ? (double)completed / nonCancelledTotal * 100 : 0;
        var cancellationRate = total > 0 ? (double)cancelled / total * 100 : 0;

        // Time-based counts
        var upcoming = allBookings.Count(b =>
            b.Status == BookingStatus.Confirmed && b.StartTimeUtc > now);
        var todaysCount = allBookings.Count(b =>
            b.StartTimeUtc.Date == today && b.Status != BookingStatus.Cancelled);
        var thisWeek = allBookings.Count(b =>
            b.StartTimeUtc >= startOfWeek && b.StartTimeUtc < startOfWeek.AddDays(7) &&
            b.Status != BookingStatus.Cancelled);
        var thisMonth = allBookings.Count(b =>
            b.StartTimeUtc >= startOfMonth && b.StartTimeUtc < startOfMonth.AddMonths(1) &&
            b.Status != BookingStatus.Cancelled);

        // Bookings per day for last 30 days
        var last30DaysBookings = allBookings
            .Where(b => b.StartTimeUtc.Date >= thirtyDaysAgo && b.Status != BookingStatus.Cancelled)
            .GroupBy(b => b.StartTimeUtc.Date)
            .Select(g => new DailyBookingCount
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Count = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToList();

        // Fill in missing days with zero
        var dailyCounts = new List<DailyBookingCount>();
        for (var date = thirtyDaysAgo; date <= today; date = date.AddDays(1))
        {
            var existing = last30DaysBookings.FirstOrDefault(d => d.Date == date.ToString("yyyy-MM-dd"));
            dailyCounts.Add(new DailyBookingCount
            {
                Date = date.ToString("yyyy-MM-dd"),
                Count = existing?.Count ?? 0
            });
        }

        // Popular hours (distribution of booking start times)
        var hourlyDistribution = allBookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .GroupBy(b => b.StartTimeUtc.Hour)
            .Select(g => new HourlyBookingCount
            {
                Hour = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(h => h.Count)
            .Take(5)
            .ToList();

        // Top event types
        var topEventTypes = allBookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .GroupBy(b => b.EventTypeId)
            .Select(g => new EventTypeBookingCount
            {
                EventTypeName = eventTypeNames.GetValueOrDefault(g.Key, "Unknown"),
                Count = g.Count()
            })
            .OrderByDescending(e => e.Count)
            .Take(5)
            .ToList();

        return Result.Success(new DashboardAnalyticsResponse
        {
            TotalBookings = total,
            ConfirmedBookings = confirmed,
            CancelledBookings = cancelled,
            CompletedBookings = completed,
            NoShowBookings = noShow,
            CompletionRate = Math.Round(completionRate, 1),
            CancellationRate = Math.Round(cancellationRate, 1),
            UpcomingBookings = upcoming,
            TodaysBookings = todaysCount,
            ThisWeeksBookings = thisWeek,
            ThisMonthsBookings = thisMonth,
            BookingsLast30Days = dailyCounts,
            PopularHours = hourlyDistribution,
            TopEventTypes = topEventTypes
        });
    }
}
