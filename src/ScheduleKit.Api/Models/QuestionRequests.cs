namespace ScheduleKit.Api.Models;

/// <summary>
/// Request model for adding a booking question.
/// </summary>
public record AddBookingQuestionRequest
{
    public string QuestionText { get; init; } = string.Empty;
    public string Type { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public List<string>? Options { get; init; }
}

/// <summary>
/// Request model for updating a booking question.
/// </summary>
public record UpdateBookingQuestionRequest
{
    public string QuestionText { get; init; } = string.Empty;
    public string Type { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public List<string>? Options { get; init; }
}

/// <summary>
/// Request model for reordering booking questions.
/// </summary>
public record ReorderQuestionsRequest
{
    public List<Guid> QuestionIds { get; init; } = new();
}
