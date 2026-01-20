using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Domain.Tests.ValueObjects;

public class DurationTests
{
    [Fact]
    public void Create_WithValidMinutes_ShouldSucceed()
    {
        // Arrange
        var minutes = 30;

        // Act
        var result = Duration.Create(minutes);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(minutes, result.Value.Minutes);
    }

    [Theory]
    [InlineData(15)]  // Minimum valid
    [InlineData(60)]  // 1 hour
    [InlineData(120)] // 2 hours
    [InlineData(480)] // Maximum valid (8 hours)
    public void Create_WithValidRangeMinutes_ShouldSucceed(int minutes)
    {
        // Act
        var result = Duration.Create(minutes);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(minutes, result.Value.Minutes);
    }

    [Theory]
    [InlineData(0)]    // Zero
    [InlineData(-1)]   // Negative
    [InlineData(14)]   // Just below minimum
    public void Create_WithMinutesBelowMinimum_ShouldFail(int minutes)
    {
        // Act
        var result = Duration.Create(minutes);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("at least 15 minutes", result.Error);
    }

    [Theory]
    [InlineData(481)]  // Just above maximum
    [InlineData(1000)] // Way above maximum
    public void Create_WithMinutesAboveMaximum_ShouldFail(int minutes)
    {
        // Act
        var result = Duration.Create(minutes);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("cannot exceed", result.Error);
    }

    [Theory]
    [InlineData(17)]   // Not divisible by 5
    [InlineData(23)]   // Not divisible by 5
    [InlineData(61)]   // Not divisible by 5
    public void Create_WithMinutesNotInFiveMinuteIncrements_ShouldFail(int minutes)
    {
        // Act
        var result = Duration.Create(minutes);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("5-minute increments", result.Error);
    }

    [Fact]
    public void Equals_WithSameMinutes_ShouldBeEqual()
    {
        // Arrange
        var duration1 = Duration.Create(30).Value;
        var duration2 = Duration.Create(30).Value;

        // Assert
        Assert.Equal(duration1, duration2);
        Assert.True(duration1 == duration2);
    }

    [Fact]
    public void Equals_WithDifferentMinutes_ShouldNotBeEqual()
    {
        // Arrange
        var duration1 = Duration.Create(30).Value;
        var duration2 = Duration.Create(60).Value;

        // Assert
        Assert.NotEqual(duration1, duration2);
        Assert.True(duration1 != duration2);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var duration = Duration.Create(30).Value;

        // Act
        var result = duration.ToString();

        // Assert
        Assert.Contains("30", result);
    }

    [Fact]
    public void ToTimeSpan_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var duration = Duration.Create(60).Value;

        // Act
        var timeSpan = duration.ToTimeSpan();

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(60), timeSpan);
    }

    [Fact]
    public void StaticDurations_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(15, Duration.FifteenMinutes.Minutes);
        Assert.Equal(30, Duration.ThirtyMinutes.Minutes);
        Assert.Equal(45, Duration.FortyFiveMinutes.Minutes);
        Assert.Equal(60, Duration.OneHour.Minutes);
        Assert.Equal(90, Duration.NinetyMinutes.Minutes);
        Assert.Equal(120, Duration.TwoHours.Minutes);
    }
}
