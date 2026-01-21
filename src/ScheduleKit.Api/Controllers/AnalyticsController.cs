using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Application.Queries.Analytics;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// API endpoints for analytics and statistics.
/// </summary>
public class AnalyticsController : ApiControllerBase
{
    /// <summary>
    /// Get dashboard analytics for the current host.
    /// </summary>
    /// <returns>Analytics data including booking stats, trends, and popular times.</returns>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardAnalyticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDashboardAnalytics()
    {
        var query = new GetDashboardAnalyticsQuery
        {
            HostUserId = GetCurrentUserId()
        };

        var result = await Mediator.Send(query);
        return ToActionResult(result);
    }
}
