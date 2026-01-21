using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;

using QuestionResponseDto = ScheduleKit.Application.Common.DTOs.BookingQuestionResponse;

namespace ScheduleKit.Application.Commands.Questions;

/// <summary>
/// Command to update a booking question.
/// </summary>
public record UpdateBookingQuestionCommand : ICommand<QuestionResponseDto>
{
    public Guid EventTypeId { get; init; }
    public Guid QuestionId { get; init; }
    public Guid HostUserId { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public string Type { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public List<string>? Options { get; init; }
}

/// <summary>
/// Validator for UpdateBookingQuestionCommand.
/// </summary>
public class UpdateBookingQuestionCommandValidator : AbstractValidator<UpdateBookingQuestionCommand>
{
    public UpdateBookingQuestionCommandValidator()
    {
        RuleFor(x => x.EventTypeId)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage("Question ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host user ID is required.");

        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required.")
            .MaximumLength(500).WithMessage("Question text must not exceed 500 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Question type is required.")
            .Must(BeValidQuestionType).WithMessage("Invalid question type.");

        RuleFor(x => x.Options)
            .Must(HaveValidOptions)
            .When(x => x.Type is "SingleSelect" or "MultiSelect")
            .WithMessage("Select questions require at least 2 options and no more than 20.");
    }

    private static bool BeValidQuestionType(string type)
    {
        return Enum.TryParse<QuestionType>(type, true, out _);
    }

    private static bool HaveValidOptions(List<string>? options)
    {
        if (options is null) return false;
        return options.Count >= 2 && options.Count <= 20 && options.All(o => !string.IsNullOrWhiteSpace(o));
    }
}

/// <summary>
/// Handler for UpdateBookingQuestionCommand.
/// </summary>
public class UpdateBookingQuestionCommandHandler : IRequestHandler<UpdateBookingQuestionCommand, Result<QuestionResponseDto>>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookingQuestionCommandHandler(
        IEventTypeRepository eventTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _eventTypeRepository = eventTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<QuestionResponseDto>> Handle(
        UpdateBookingQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var eventType = await _eventTypeRepository.GetByIdAsync(request.EventTypeId, cancellationToken);
        if (eventType is null)
        {
            return Result.Failure<QuestionResponseDto>("Event type not found.");
        }

        if (eventType.HostUserId != request.HostUserId)
        {
            return Result.Failure<QuestionResponseDto>("You do not have permission to modify this event type.");
        }

        var question = eventType.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
        if (question is null)
        {
            return Result.Failure<QuestionResponseDto>("Question not found.");
        }

        if (!Enum.TryParse<QuestionType>(request.Type, true, out var questionType))
        {
            return Result.Failure<QuestionResponseDto>("Invalid question type.");
        }

        var updateResult = question.Update(
            request.QuestionText,
            questionType,
            request.IsRequired,
            request.Options);

        if (updateResult.IsFailure)
        {
            return Result.Failure<QuestionResponseDto>(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new QuestionResponseDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            Type = question.Type.ToString(),
            IsRequired = question.IsRequired,
            Options = question.Options,
            DisplayOrder = question.DisplayOrder
        });
    }
}
