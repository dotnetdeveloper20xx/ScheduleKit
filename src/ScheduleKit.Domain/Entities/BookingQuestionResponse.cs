using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.Entities;

/// <summary>
/// Represents a guest's response to a booking question.
/// </summary>
public class BookingQuestionResponse : BaseEntity
{
    public Guid BookingId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string ResponseValue { get; private set; } = string.Empty;

    // For EF Core
    private BookingQuestionResponse() { }

    private BookingQuestionResponse(Guid bookingId, Guid questionId, string responseValue)
    {
        Id = Guid.NewGuid();
        BookingId = bookingId;
        QuestionId = questionId;
        ResponseValue = responseValue;
        SetCreatedAt();
    }

    /// <summary>
    /// Creates a new question response.
    /// </summary>
    public static Result<BookingQuestionResponse> Create(
        Guid bookingId,
        Guid questionId,
        string responseValue)
    {
        if (bookingId == Guid.Empty)
            return Result.Failure<BookingQuestionResponse>("Booking ID is required.");

        if (questionId == Guid.Empty)
            return Result.Failure<BookingQuestionResponse>("Question ID is required.");

        if (responseValue?.Length > 5000)
            return Result.Failure<BookingQuestionResponse>("Response is too long.");

        return Result.Success(new BookingQuestionResponse(
            bookingId,
            questionId,
            responseValue?.Trim() ?? string.Empty));
    }
}
