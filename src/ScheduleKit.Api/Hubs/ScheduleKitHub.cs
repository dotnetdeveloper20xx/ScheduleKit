using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ScheduleKit.Application.Common;

namespace ScheduleKit.Api.Hubs;

/// <summary>
/// SignalR hub for real-time scheduling updates.
/// </summary>
public class ScheduleKitHub : Hub<IScheduleKitClient>
{
    private readonly ILogger<ScheduleKitHub> _logger;

    public ScheduleKitHub(ILogger<ScheduleKitHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Guests join an event type group to receive slot availability updates.
    /// </summary>
    public async Task JoinEventTypeGroup(Guid eventTypeId)
    {
        var groupName = GetEventTypeGroupName(eventTypeId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("Connection {ConnectionId} joined event type group {GroupName}",
            Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Guests leave an event type group.
    /// </summary>
    public async Task LeaveEventTypeGroup(Guid eventTypeId)
    {
        var groupName = GetEventTypeGroupName(eventTypeId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("Connection {ConnectionId} left event type group {GroupName}",
            Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Authenticated hosts join their dashboard group for booking updates.
    /// </summary>
    [Authorize]
    public async Task JoinHostDashboard()
    {
        var currentUserService = Context.GetHttpContext()?.RequestServices
            .GetService<ICurrentUserService>();

        if (currentUserService == null || !currentUserService.IsAuthenticated)
        {
            throw new HubException("Unauthorized");
        }

        var groupName = GetHostGroupName(currentUserService.UserId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("Connection {ConnectionId} joined host dashboard {GroupName}",
            Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Hosts leave their dashboard group.
    /// </summary>
    [Authorize]
    public async Task LeaveHostDashboard()
    {
        var currentUserService = Context.GetHttpContext()?.RequestServices
            .GetService<ICurrentUserService>();

        if (currentUserService == null || !currentUserService.IsAuthenticated)
        {
            return;
        }

        var groupName = GetHostGroupName(currentUserService.UserId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("Connection {ConnectionId} left host dashboard {GroupName}",
            Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogDebug("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("Client disconnected: {ConnectionId}, Error: {Error}",
            Context.ConnectionId, exception?.Message);
        await base.OnDisconnectedAsync(exception);
    }

    public static string GetEventTypeGroupName(Guid eventTypeId) => $"event:{eventTypeId}";
    public static string GetHostGroupName(Guid hostUserId) => $"host:{hostUserId}";
}
