using MediatR;
using ScheduleKit.Domain.Common;

namespace ScheduleKit.Application.Common;

/// <summary>
/// Marker interface for commands (write operations).
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// Marker interface for commands that return a value.
/// </summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

/// <summary>
/// Marker interface for queries (read operations).
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

/// <summary>
/// Interface for current user service.
/// </summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
