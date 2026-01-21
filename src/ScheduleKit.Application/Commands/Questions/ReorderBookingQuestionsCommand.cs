using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Questions;

/// <summary>
/// Command to reorder booking questions.
/// </summary>
public record ReorderBookingQuestionsCommand : ICommand
{
    public Guid EventTypeId { get; init; }
    public Guid HostUserId { get; init; }
    public List<Guid> QuestionIds { get; init; } = new();
}

/// <summary>
/// Validator for ReorderBookingQuestionsCommand.
/// </summary>
public class ReorderBookingQuestionsCommandValidator : AbstractValidator<ReorderBookingQuestionsCommand>
{
    public ReorderBookingQuestionsCommandValidator()
    {
        RuleFor(x => x.EventTypeId)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");

        RuleFor(x => x.QuestionIds)
            .NotEmpty().WithMessage("Question IDs are required.");
    }
}

/// <summary>
/// Handler for ReorderBookingQuestionsCommand.
/// </summary>
public class ReorderBookingQuestionsCommandHandler : IRequestHandler<ReorderBookingQuestionsCommand, Result>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderBookingQuestionsCommandHandler(
        IEventTypeRepository eventTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _eventTypeRepository = eventTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ReorderBookingQuestionsCommand request,
        CancellationToken cancellationToken)
    {
        var eventType = await _eventTypeRepository.GetByIdAsync(request.EventTypeId, cancellationToken);
        if (eventType is null)
        {
            return Result.Failure("Event type not found.");
        }

        if (eventType.HostUserId != request.HostUserId)
        {
            return Result.Failure("You do not have permission to modify this event type.");
        }

        var reorderResult = eventType.ReorderQuestions(request.QuestionIds);
        if (reorderResult.IsFailure)
        {
            return reorderResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
