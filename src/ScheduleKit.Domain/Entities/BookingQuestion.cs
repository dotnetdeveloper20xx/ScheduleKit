using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.Entities;

/// <summary>
/// Type of question asked during booking.
/// </summary>
public enum QuestionType
{
    Text,
    MultilineText,
    SingleSelect,
    MultiSelect,
    Checkbox
}

/// <summary>
/// Represents a custom question asked during the booking process.
/// </summary>
public class BookingQuestion : BaseEntity
{
    public Guid EventTypeId { get; private set; }
    public string QuestionText { get; private set; } = string.Empty;
    public QuestionType Type { get; private set; }
    public bool IsRequired { get; private set; }
    public List<string> Options { get; private set; } = new();
    public int DisplayOrder { get; private set; }

    // For EF Core
    private BookingQuestion() { }

    private BookingQuestion(
        Guid eventTypeId,
        string questionText,
        QuestionType type,
        bool isRequired,
        List<string>? options)
    {
        Id = Guid.NewGuid();
        EventTypeId = eventTypeId;
        QuestionText = questionText;
        Type = type;
        IsRequired = isRequired;
        Options = options ?? new List<string>();
        SetCreatedAt();
    }

    /// <summary>
    /// Creates a new booking question.
    /// </summary>
    public static Result<BookingQuestion> Create(
        Guid eventTypeId,
        string questionText,
        QuestionType type,
        bool isRequired,
        List<string>? options = null)
    {
        if (eventTypeId == Guid.Empty)
            return Result.Failure<BookingQuestion>("Event type ID is required.");

        if (string.IsNullOrWhiteSpace(questionText))
            return Result.Failure<BookingQuestion>("Question text is required.");

        if (questionText.Length > 500)
            return Result.Failure<BookingQuestion>("Question text is too long.");

        // Validate options for select types
        if (type is QuestionType.SingleSelect or QuestionType.MultiSelect)
        {
            if (options is null || options.Count < 2)
                return Result.Failure<BookingQuestion>("Select questions require at least 2 options.");

            if (options.Count > 20)
                return Result.Failure<BookingQuestion>("Maximum of 20 options allowed.");

            if (options.Any(string.IsNullOrWhiteSpace))
                return Result.Failure<BookingQuestion>("Options cannot be empty.");
        }

        return Result.Success(new BookingQuestion(
            eventTypeId,
            questionText.Trim(),
            type,
            isRequired,
            options?.Select(o => o.Trim()).ToList()));
    }

    /// <summary>
    /// Updates the question details.
    /// </summary>
    public Result Update(string questionText, QuestionType type, bool isRequired, List<string>? options)
    {
        if (string.IsNullOrWhiteSpace(questionText))
            return Result.Failure("Question text is required.");

        if (questionText.Length > 500)
            return Result.Failure("Question text is too long.");

        if (type is QuestionType.SingleSelect or QuestionType.MultiSelect)
        {
            if (options is null || options.Count < 2)
                return Result.Failure("Select questions require at least 2 options.");

            if (options.Count > 20)
                return Result.Failure("Maximum of 20 options allowed.");
        }

        QuestionText = questionText.Trim();
        Type = type;
        IsRequired = isRequired;
        Options = options?.Select(o => o.Trim()).ToList() ?? new List<string>();
        SetUpdatedAt();

        return Result.Success();
    }

    internal void SetDisplayOrder(int order)
    {
        DisplayOrder = order;
    }
}
