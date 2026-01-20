using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Application.Common.Mappings;

/// <summary>
/// Extension methods for mapping EventType entities to DTOs.
/// </summary>
public static class EventTypeMappings
{
    public static EventTypeResponse ToResponse(this EventType eventType)
    {
        return new EventTypeResponse
        {
            Id = eventType.Id,
            HostUserId = eventType.HostUserId,
            Name = eventType.Name,
            Slug = eventType.Slug.Value,
            Description = eventType.Description,
            DurationMinutes = eventType.Duration.Minutes,
            BufferBeforeMinutes = eventType.BufferBefore.Minutes,
            BufferAfterMinutes = eventType.BufferAfter.Minutes,
            LocationType = eventType.Location.Type.ToString(),
            LocationDetails = eventType.Location.Details,
            LocationDisplayName = eventType.Location.DisplayName,
            IsActive = eventType.IsActive,
            Color = eventType.Color,
            CreatedAtUtc = eventType.CreatedAtUtc,
            UpdatedAtUtc = eventType.UpdatedAtUtc,
            Questions = eventType.Questions.Select(q => q.ToResponse()).ToList()
        };
    }

    public static DTOs.BookingQuestionResponse ToResponse(this BookingQuestion question)
    {
        return new DTOs.BookingQuestionResponse
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            Type = question.Type.ToString(),
            IsRequired = question.IsRequired,
            Options = question.Options,
            DisplayOrder = question.DisplayOrder
        };
    }

    public static List<EventTypeResponse> ToResponseList(this IEnumerable<EventType> eventTypes)
    {
        return eventTypes.Select(e => e.ToResponse()).ToList();
    }
}
