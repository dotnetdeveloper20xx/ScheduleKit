using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Application.Commands.Questions;

/// <summary>
/// Command to delete a booking question.
/// </summary>
public record DeleteBookingQuestionCommand : ICommand
{
    public Guid EventTypeId { get; init; }
    public Guid QuestionId { get; init; }
    public Guid HostUserId { get; init; }
}

/// <summary>
/// Validator for DeleteBookingQuestionCommand.
/// </summary>
public class DeleteBookingQuestionCommandValidator : AbstractValidator<DeleteBookingQuestionCommand>
{
    public DeleteBookingQuestionCommandValidator()
    {
        RuleFor(x => x.EventTypeId)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage("Question ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");
    }
}

/// <summary>
/// Handler for DeleteBookingQuestionCommand.
/// </summary>
public class DeleteBookingQuestionCommandHandler : IRequestHandler<DeleteBookingQuestionCommand, Result>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBookingQuestionCommandHandler(
        IEventTypeRepository eventTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _eventTypeRepository = eventTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteBookingQuestionCommand request,
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

        var removeResult = eventType.RemoveQuestion(request.QuestionId);
        if (removeResult.IsFailure)
        {
            return removeResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
