using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Domain.Tests.Entities;

public class EventTypeTests
{
    private readonly Guid _hostUserId = Guid.NewGuid();
    private readonly MeetingLocation _defaultLocation = MeetingLocation.CreateZoom();

    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var name = "30 Minute Meeting";
        var durationMinutes = 30;

        // Act
        var result = EventType.Create(
            _hostUserId,
            name,
            durationMinutes,
            _defaultLocation);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(durationMinutes, result.Value.Duration.Minutes);
        Assert.Equal(_hostUserId, result.Value.HostUserId);
        Assert.True(result.Value.IsActive);
    }

    [Fact]
    public void Create_ShouldGenerateSlugFromName()
    {
        // Arrange
        var name = "30 Minute Meeting";

        // Act
        var result = EventType.Create(
            _hostUserId,
            name,
            30,
            _defaultLocation);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("30-minute-meeting", result.Value.Slug.Value);
    }

    [Fact]
    public void Create_WithEmptyHostUserId_ShouldFail()
    {
        // Act
        var result = EventType.Create(
            Guid.Empty,
            "Meeting",
            30,
            _defaultLocation);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Host user ID", result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldFail(string? name)
    {
        // Act
        var result = EventType.Create(
            _hostUserId,
            name!,
            30,
            _defaultLocation);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("name", result.Error.ToLower());
    }

    [Fact]
    public void Create_WithOptionalParameters_ShouldSetThem()
    {
        // Arrange
        var description = "A quick sync call";
        var bufferBefore = 5;
        var bufferAfter = 10;
        var minimumNotice = 120;
        var bookingWindow = 30;
        int? maxBookingsPerDay = 5;
        var color = "#FF5733";

        // Act
        var result = EventType.Create(
            _hostUserId,
            "Quick Call",
            30,
            _defaultLocation,
            description,
            bufferBefore,
            bufferAfter,
            minimumNotice,
            bookingWindow,
            maxBookingsPerDay,
            color);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(bufferBefore, result.Value.BufferBefore.Minutes);
        Assert.Equal(bufferAfter, result.Value.BufferAfter.Minutes);
        Assert.Equal(minimumNotice, result.Value.MinimumNotice.Minutes);
        Assert.Equal(bookingWindow, result.Value.BookingWindow.Days);
        Assert.Equal(maxBookingsPerDay, result.Value.MaxBookingsPerDay);
        Assert.Equal(color, result.Value.Color);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var eventType = EventType.Create(
            _hostUserId,
            "Original Name",
            30,
            _defaultLocation).Value;

        var newName = "Updated Name";
        var newDescription = "Updated description";

        // Act
        var result = eventType.UpdateDetails(
            newName,
            newDescription,
            60,
            5,
            10,
            120,  // minimumNoticeMinutes
            30,   // bookingWindowDays
            5,    // maxBookingsPerDay
            MeetingLocation.CreateGoogleMeet(),
            "#00FF00");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, eventType.Name);
        Assert.Equal(newDescription, eventType.Description);
        Assert.Equal(60, eventType.Duration.Minutes);
        Assert.Equal(120, eventType.MinimumNotice.Minutes);
        Assert.Equal(30, eventType.BookingWindow.Days);
        Assert.Equal(5, eventType.MaxBookingsPerDay);
    }

    [Fact]
    public void UpdateSlug_ShouldUpdateSlug()
    {
        // Arrange
        var eventType = EventType.Create(
            _hostUserId,
            "Meeting",
            30,
            _defaultLocation).Value;

        var newSlug = "custom-slug-123";

        // Act
        eventType.UpdateSlug(newSlug);

        // Assert
        Assert.Equal(newSlug, eventType.Slug.Value);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var eventType = EventType.Create(
            _hostUserId,
            "Meeting",
            30,
            _defaultLocation).Value;

        eventType.Deactivate();

        // Act
        eventType.Activate();

        // Assert
        Assert.True(eventType.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var eventType = EventType.Create(
            _hostUserId,
            "Meeting",
            30,
            _defaultLocation).Value;

        // Act
        eventType.Deactivate();

        // Assert
        Assert.False(eventType.IsActive);
    }

    [Fact]
    public void AddQuestion_ShouldAddToQuestionsList()
    {
        // Arrange
        var eventType = EventType.Create(
            _hostUserId,
            "Meeting",
            30,
            _defaultLocation).Value;

        var question = BookingQuestion.Create(
            eventType.Id,
            "What is your goal?",
            QuestionType.Text,
            true).Value;

        // Act
        eventType.AddQuestion(question);

        // Assert
        Assert.Contains(question, eventType.Questions);
    }

    [Fact]
    public void RemoveQuestion_ShouldRemoveFromQuestionsList()
    {
        // Arrange
        var eventType = EventType.Create(
            _hostUserId,
            "Meeting",
            30,
            _defaultLocation).Value;

        var question = BookingQuestion.Create(
            eventType.Id,
            "What is your goal?",
            QuestionType.Text,
            true).Value;

        eventType.AddQuestion(question);

        // Act
        eventType.RemoveQuestion(question.Id);

        // Assert
        Assert.DoesNotContain(question, eventType.Questions);
    }
}
