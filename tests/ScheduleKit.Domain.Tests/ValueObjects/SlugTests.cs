using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Domain.Tests.ValueObjects;

public class SlugTests
{
    [Theory]
    [InlineData("30-minute-meeting")]
    [InlineData("quick-call")]
    [InlineData("consultation-session")]
    public void Create_WithValidSlug_ShouldSucceed(string slugValue)
    {
        // Act
        var result = Slug.Create(slugValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(slugValue, result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithEmptySlug_ShouldFail(string? slugValue)
    {
        // Act
        var result = Slug.Create(slugValue!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("required", result.Error);
    }

    [Theory]
    [InlineData("ab")]  // Too short (min is 3)
    public void Create_WithTooShortSlug_ShouldFail(string slugValue)
    {
        // Act
        var result = Slug.Create(slugValue);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("at least", result.Error);
    }

    [Fact]
    public void Create_WithSlugStartingWithHyphen_ShouldFail()
    {
        // Act
        var result = Slug.Create("-invalid");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("hyphen", result.Error.ToLower());
    }

    [Fact]
    public void Create_WithSlugEndingWithHyphen_ShouldFail()
    {
        // Act
        var result = Slug.Create("invalid-");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("hyphen", result.Error.ToLower());
    }

    [Fact]
    public void Create_WithConsecutiveHyphens_ShouldFail()
    {
        // Act
        var result = Slug.Create("invalid--slug");

        // Assert
        Assert.True(result.IsFailure);
        // The error could be about invalid characters or consecutive hyphens
        Assert.True(result.Error.ToLower().Contains("hyphen") || result.Error.ToLower().Contains("only contain"));
    }

    [Theory]
    [InlineData("30 Minute Meeting", "30-minute-meeting")]
    [InlineData("Quick Call!", "quick-call")]
    [InlineData("Consultation & Planning", "consultation-planning")]
    [InlineData("Team Meeting (Weekly)", "team-meeting-weekly")]
    public void FromName_ShouldGenerateValidSlug(string name, string expectedSlug)
    {
        // Act
        var result = Slug.FromName(name);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedSlug, result.Value.Value);
    }

    [Fact]
    public void FromName_WithLongName_ShouldTruncate()
    {
        // Arrange
        var longName = string.Join(" ", Enumerable.Repeat("word", 50));

        // Act
        var result = Slug.FromName(longName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Value.Length <= 100);
    }

    [Fact]
    public void Equals_WithSameSlug_ShouldBeEqual()
    {
        // Arrange
        var slug1 = Slug.Create("meeting").Value;
        var slug2 = Slug.Create("meeting").Value;

        // Assert
        Assert.Equal(slug1, slug2);
        Assert.True(slug1 == slug2);
    }

    [Fact]
    public void Equals_WithDifferentSlug_ShouldNotBeEqual()
    {
        // Arrange
        var slug1 = Slug.Create("meeting").Value;
        var slug2 = Slug.Create("call").Value;

        // Assert
        Assert.NotEqual(slug1, slug2);
        Assert.True(slug1 != slug2);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var slug = Slug.Create("my-meeting").Value;

        // Act
        var result = slug.ToString();

        // Assert
        Assert.Equal("my-meeting", result);
    }

    [Fact]
    public void Create_ShouldNormalizeToLowercase()
    {
        // Act
        var result = Slug.Create("MY-MEETING");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("my-meeting", result.Value.Value);
    }
}
