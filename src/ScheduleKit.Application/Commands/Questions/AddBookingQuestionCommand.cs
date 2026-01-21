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
/// Command to add a booking question to an event type.
/// </summary>
public record AddBookingQuestionCommand : ICommand<QuestionResponseDto>
{
    public Guid EventTypeId { get; init; }
    public Guid HostUserId { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public string Type { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public List<string>? Options { get; init; }
}

/// <summary>
/// Validator for AddBookingQuestionCommand.
/// </summary>
public class AddBookingQuestionCommandValidator : AbstractValidator<AddBookingQuestionCommand>
{
    public AddBookingQuestionCommandValidator()
    {
        RuleFor(x => x.EventTypeId)
            .NotEmpty().WithMessage("Event type ID is required.");

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
/// Handler for AddBookingQuestionCommand.
/// </summary>
public class AddBookingQuestionCommandHandler : IRequestHandler<AddBookingQuestionCommand, Result<QuestionResponseDto>>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddBookingQuestionCommandHandler(
        IEventTypeRepository eventTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _eventTypeRepository = eventTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<QuestionResponseDto>> Handle(
        AddBookingQuestionCommand request,
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

        if (!Enum.TryParse<QuestionType>(request.Type, true, out var questionType))
        {
            return Result.Failure<QuestionResponseDto>("Invalid question type.");
        }

        var questionResult = BookingQuestion.Create(
            request.EventTypeId,
            request.QuestionText,
            questionType,
            request.IsRequired,
            request.Options);

        if (questionResult.IsFailure)
        {
            return Result.Failure<QuestionResponseDto>(questionResult.Error);
        }

        var addResult = eventType.AddQuestion(questionResult.Value);
        if (addResult.IsFailure)
        {
            return Result.Failure<QuestionResponseDto>(addResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new QuestionResponseDto
        {
            Id = questionResult.Value.Id,
            QuestionText = questionResult.Value.QuestionText,
            Type = questionResult.Value.Type.ToString(),
            IsRequired = questionResult.Value.IsRequired,
            Options = questionResult.Value.Options,
            DisplayOrder = questionResult.Value.DisplayOrder
        });
    }
}
