using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("first.last+tag@company.org")]
    public void Create_WithValidEmail_ShouldSucceed(string emailAddress)
    {
        // Act
        var result = Email.Create(emailAddress);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(emailAddress.ToLowerInvariant(), result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithEmptyEmail_ShouldFail(string? emailAddress)
    {
        // Act
        var result = Email.Create(emailAddress!);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    [InlineData("spaces in@email.com")]
    public void Create_WithInvalidFormat_ShouldFail(string emailAddress)
    {
        // Act
        var result = Email.Create(emailAddress);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Invalid email", result.Error);
    }

    [Fact]
    public void Create_ShouldNormalizeToLowercase()
    {
        // Arrange
        var emailAddress = "Test@EXAMPLE.COM";

        // Act
        var result = Email.Create(emailAddress);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("test@example.com", result.Value.Value);
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.Create("test@example.com").Value;
        var email2 = Email.Create("TEST@EXAMPLE.COM").Value;

        // Assert
        Assert.Equal(email1, email2);
    }

    [Fact]
    public void GetHashCode_WithSameEmail_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.Create("test@example.com").Value;
        var email2 = Email.Create("test@example.com").Value;

        // Assert
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }
}
