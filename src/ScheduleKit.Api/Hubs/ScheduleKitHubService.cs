using Microsoft.AspNetCore.SignalR;
using ScheduleKit.Application.Common.Interfaces;

namespace ScheduleKit.Api.Hubs;

/// <summary>
/// Implementation of the real-time notification service using SignalR.
/// </summary>
public class ScheduleKitHubService : IRealTimeNotificationService
{
    private readonly IHubContext<ScheduleKitHub, IScheduleKitClient> _hubContext;
    private readonly ILogger<ScheduleKitHubService> _logger;

    public ScheduleKitHubService(
        IHubContext<ScheduleKitHub, IScheduleKitClient> hubContext,
        ILogger<ScheduleKitHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifySlotBookedAsync(Guid eventTypeId, DateTime startTimeUtc, CancellationToken ct = default)
    {
        var groupName = ScheduleKitHub.GetEventTypeGroupName(eventTypeId);
        await _hubContext.Clients.Group(groupName).SlotBooked(eventTypeId, startTimeUtc);
        _logger.LogDebug("Notified slot booked for event type {EventTypeId} at {StartTime}",
            eventTypeId, startTimeUtc);
    }

    public async Task NotifySlotReleasedAsync(Guid eventTypeId, DateTime startTimeUtc, CancellationToken ct = default)
    {
        var groupName = ScheduleKitHub.GetEventTypeGroupName(eventTypeId);
        await _hubContext.Clients.Group(groupName).SlotReleased(eventTypeId, startTimeUtc);
        _logger.LogDebug("Notified slot released for event type {EventTypeId} at {StartTime}",
            eventTypeId, startTimeUtc);
    }

    public async Task NotifyBookingCreatedAsync(BookingCreatedNotification notification, CancellationToken ct = default)
    {
        // Notify event type watchers about slot being taken
        var eventGroupName = ScheduleKitHub.GetEventTypeGroupName(notification.EventTypeId);
        await _hubContext.Clients.Group(eventGroupName).SlotBooked(notification.EventTypeId, notification.StartTimeUtc);

        // Notify host dashboard
        var hostGroupName = ScheduleKitHub.GetHostGroupName(notification.HostUserId);
        await _hubContext.Clients.Group(hostGroupName).BookingCreated(new BookingNotification
        {
            Id = notification.BookingId,
            EventTypeId = notification.EventTypeId,
            EventTypeName = notification.EventTypeName,
            GuestName = notification.GuestName,
            GuestEmail = notification.GuestEmail,
            StartTimeUtc = notification.StartTimeUtc,
            EndTimeUtc = notification.EndTimeUtc,
            GuestTimezone = notification.GuestTimezone,
            CreatedAtUtc = notification.CreatedAtUtc
        });

        _logger.LogDebug("Notified host {HostUserId} of new booking {BookingId}",
            notification.HostUserId, notification.BookingId);
    }

    public async Task NotifyBookingCancelledAsync(Guid hostUserId, Guid bookingId, string? reason, CancellationToken ct = default)
    {
        var groupName = ScheduleKitHub.GetHostGroupName(hostUserId);
        await _hubContext.Clients.Group(groupName).BookingCancelled(bookingId, reason);
        _logger.LogDebug("Notified host {HostUserId} of cancelled booking {BookingId}",
            hostUserId, bookingId);
    }

    public async Task NotifyBookingRescheduledAsync(Guid hostUserId, Guid eventTypeId, Guid bookingId,
        DateTime oldStartTimeUtc, DateTime newStartTimeUtc, CancellationToken ct = default)
    {
        // Notify host dashboard
        var hostGroupName = ScheduleKitHub.GetHostGroupName(hostUserId);
        await _hubContext.Clients.Group(hostGroupName).BookingRescheduled(bookingId, oldStartTimeUtc, newStartTimeUtc);

        // Notify event type watchers about slot changes
        var eventGroupName = ScheduleKitHub.GetEventTypeGroupName(eventTypeId);
        await _hubContext.Clients.Group(eventGroupName).SlotReleased(eventTypeId, oldStartTimeUtc);
        await _hubContext.Clients.Group(eventGroupName).SlotBooked(eventTypeId, newStartTimeUtc);

        _logger.LogDebug("Notified host {HostUserId} of rescheduled booking {BookingId}",
            hostUserId, bookingId);
    }
}
