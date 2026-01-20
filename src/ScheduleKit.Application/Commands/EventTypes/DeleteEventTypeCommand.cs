using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.EventTypes;

/// <summary>
/// Command to delete an event type.
/// </summary>
public record DeleteEventTypeCommand : ICommand
{
    public Guid Id { get; init; }
    public Guid HostUserId { get; init; }
}

/// <summary>
/// Validator for DeleteEventTypeCommand.
/// </summary>
public class DeleteEventTypeCommandValidator : AbstractValidator<DeleteEventTypeCommand>
{
    public DeleteEventTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");
    }
}

/// <summary>
/// Handler for DeleteEventTypeCommand.
/// </summary>
public class DeleteEventTypeCommandHandler : IRequestHandler<DeleteEventTypeCommand, Result>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventTypeCommandHandler(
        IEventTypeRepository eventTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _eventTypeRepository = eventTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEventTypeCommand request, CancellationToken cancellationToken)
    {
        var eventType = await _eventTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (eventType is null)
        {
            return Result.Failure("Event type not found.");
        }

        // Verify ownership
        if (eventType.HostUserId != request.HostUserId)
        {
            return Result.Failure("You do not have permission to delete this event type.");
        }

        _eventTypeRepository.Remove(eventType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
