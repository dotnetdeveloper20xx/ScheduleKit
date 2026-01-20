using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Events;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Domain.Entities;

/// <summary>
/// Represents a type of meeting that can be booked.
/// Examples: "30-minute Consultation", "1-hour Strategy Session"
/// This is an aggregate root.
/// </summary>
public class EventType : BaseEntity, IAggregateRoot
{
    private readonly List<BookingQuestion> _questions = new();

    public Guid HostUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Slug Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public Duration Duration { get; private set; } = null!;
    public BufferTime BufferBefore { get; private set; } = null!;
    public BufferTime BufferAfter { get; private set; } = null!;
    public MeetingLocation Location { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public string? Color { get; private set; }

    public IReadOnlyCollection<BookingQuestion> Questions => _questions.AsReadOnly();

    // For EF Core
    private EventType() { }

    private EventType(
        Guid hostUserId,
        string name,
        Slug slug,
        string? description,
        Duration duration,
        BufferTime bufferBefore,
        BufferTime bufferAfter,
        MeetingLocation location,
        string? color)
    {
        Id = Guid.NewGuid();
        HostUserId = hostUserId;
        Name = name;
        Slug = slug;
        Description = description;
        Duration = duration;
        BufferBefore = bufferBefore;
        BufferAfter = bufferAfter;
        Location = location;
        IsActive = true;
        Color = color;
        SetCreatedAt();

        AddDomainEvent(new EventTypeCreatedEvent(Id, HostUserId, Name));
    }

    /// <summary>
    /// Creates a new EventType with validation.
    /// </summary>
    public static Result<EventType> Create(
        Guid hostUserId,
        string name,
        int durationMinutes,
        MeetingLocation location,
        string? description = null,
        int bufferBeforeMinutes = 0,
        int bufferAfterMinutes = 0,
        string? color = null)
    {
        if (hostUserId == Guid.Empty)
            return Result.Failure<EventType>("Host user ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<EventType>("Event type name is required.");

        if (name.Length > 200)
            return Result.Failure<EventType>("Event type name is too long.");

        var slugResult = Slug.FromName(name);
        if (slugResult.IsFailure)
            return Result.Failure<EventType>(slugResult.Error);

        var durationResult = Duration.Create(durationMinutes);
        if (durationResult.IsFailure)
            return Result.Failure<EventType>(durationResult.Error);

        var bufferBeforeResult = BufferTime.Create(bufferBeforeMinutes);
        if (bufferBeforeResult.IsFailure)
            return Result.Failure<EventType>(bufferBeforeResult.Error);

        var bufferAfterResult = BufferTime.Create(bufferAfterMinutes);
        if (bufferAfterResult.IsFailure)
            return Result.Failure<EventType>(bufferAfterResult.Error);

        if (description?.Length > 2000)
            return Result.Failure<EventType>("Description is too long.");

        return Result.Success(new EventType(
            hostUserId,
            name.Trim(),
            slugResult.Value,
            description?.Trim(),
            durationResult.Value,
            bufferBeforeResult.Value,
            bufferAfterResult.Value,
            location,
            color));
    }

    /// <summary>
    /// Updates the event type details.
    /// </summary>
    public Result UpdateDetails(
        string name,
        string? description,
        int durationMinutes,
        int bufferBeforeMinutes,
        int bufferAfterMinutes,
        MeetingLocation location,
        string? color)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Event type name is required.");

        if (name.Length > 200)
            return Result.Failure("Event type name is too long.");

        var durationResult = Duration.Create(durationMinutes);
        if (durationResult.IsFailure)
            return Result.Failure(durationResult.Error);

        var bufferBeforeResult = BufferTime.Create(bufferBeforeMinutes);
        if (bufferBeforeResult.IsFailure)
            return Result.Failure(bufferBeforeResult.Error);

        var bufferAfterResult = BufferTime.Create(bufferAfterMinutes);
        if (bufferAfterResult.IsFailure)
            return Result.Failure(bufferAfterResult.Error);

        if (description?.Length > 2000)
            return Result.Failure("Description is too long.");

        // Only regenerate slug if name changed
        if (Name != name.Trim())
        {
            var slugResult = Slug.FromName(name);
            if (slugResult.IsFailure)
                return Result.Failure(slugResult.Error);
            Slug = slugResult.Value;
        }

        Name = name.Trim();
        Description = description?.Trim();
        Duration = durationResult.Value;
        BufferBefore = bufferBeforeResult.Value;
        BufferAfter = bufferAfterResult.Value;
        Location = location;
        Color = color;
        SetUpdatedAt();

        AddDomainEvent(new EventTypeUpdatedEvent(Id, Name));

        return Result.Success();
    }

    /// <summary>
    /// Updates just the slug (useful for making unique slugs).
    /// </summary>
    public Result UpdateSlug(string slug)
    {
        var slugResult = Slug.Create(slug);
        if (slugResult.IsFailure)
            return Result.Failure(slugResult.Error);

        Slug = slugResult.Value;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Activates the event type so it can receive bookings.
    /// </summary>
    public Result Activate()
    {
        if (IsActive)
            return Result.Failure("Event type is already active.");

        IsActive = true;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Deactivates the event type. It will no longer accept bookings.
    /// </summary>
    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure("Event type is already inactive.");

        IsActive = false;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Adds a booking question to this event type.
    /// </summary>
    public Result AddQuestion(BookingQuestion question)
    {
        if (_questions.Count >= 10)
            return Result.Failure("Maximum of 10 questions allowed per event type.");

        question.SetDisplayOrder(_questions.Count);
        _questions.Add(question);
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Removes a booking question.
    /// </summary>
    public Result RemoveQuestion(Guid questionId)
    {
        var question = _questions.FirstOrDefault(q => q.Id == questionId);
        if (question is null)
            return Result.Failure("Question not found.");

        _questions.Remove(question);
        ReorderQuestions();
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Reorders questions based on provided IDs.
    /// </summary>
    public Result ReorderQuestions(IEnumerable<Guid> orderedIds)
    {
        var idList = orderedIds.ToList();

        if (idList.Count != _questions.Count)
            return Result.Failure("Must provide all question IDs.");

        if (!idList.All(id => _questions.Any(q => q.Id == id)))
            return Result.Failure("Invalid question ID in list.");

        for (int i = 0; i < idList.Count; i++)
        {
            var question = _questions.First(q => q.Id == idList[i]);
            question.SetDisplayOrder(i);
        }

        SetUpdatedAt();
        return Result.Success();
    }

    private void ReorderQuestions()
    {
        var ordered = _questions.OrderBy(q => q.DisplayOrder).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].SetDisplayOrder(i);
        }
    }

    /// <summary>
    /// Gets the total time blocked for a booking (duration + buffers).
    /// </summary>
    public TimeSpan GetTotalBlockedTime()
    {
        return Duration.ToTimeSpan() + BufferBefore.ToTimeSpan() + BufferAfter.ToTimeSpan();
    }
}
