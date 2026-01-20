using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Domain.Tests.ValueObjects;

public class MeetingLocationTests
{
    [Fact]
    public void CreateZoom_ShouldCreateValidLocation()
    {
        // Act
        var location = MeetingLocation.CreateZoom();

        // Assert
        Assert.Equal(LocationType.Zoom, location.Type);
        Assert.Equal("Zoom", location.DisplayName);
    }

    [Fact]
    public void CreateZoom_WithCustomLink_ShouldSetDetails()
    {
        // Arrange
        var zoomLink = "https://zoom.us/j/123456789";

        // Act
        var location = MeetingLocation.CreateZoom(zoomLink);

        // Assert
        Assert.Equal(LocationType.Zoom, location.Type);
        Assert.Equal(zoomLink, location.Details);
    }

    [Fact]
    public void CreateGoogleMeet_ShouldCreateValidLocation()
    {
        // Act
        var location = MeetingLocation.CreateGoogleMeet();

        // Assert
        Assert.Equal(LocationType.GoogleMeet, location.Type);
        Assert.Equal("Google Meet", location.DisplayName);
    }

    [Fact]
    public void CreateMicrosoftTeams_ShouldCreateValidLocation()
    {
        // Act
        var location = MeetingLocation.CreateMicrosoftTeams();

        // Assert
        Assert.Equal(LocationType.MicrosoftTeams, location.Type);
        Assert.Equal("Microsoft Teams", location.DisplayName);
    }

    [Theory]
    [InlineData("+1234567890")]
    [InlineData("+44 20 7946 0958")]
    public void CreatePhone_WithValidNumber_ShouldSucceed(string phoneNumber)
    {
        // Act
        var result = MeetingLocation.CreatePhone(phoneNumber);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(LocationType.Phone, result.Value.Type);
        Assert.Equal(phoneNumber, result.Value.Details);
    }

    [Fact]
    public void CreatePhone_WithEmptyNumber_ShouldFail()
    {
        // Act
        var result = MeetingLocation.CreatePhone("");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("123 Main St, City")]
    [InlineData("Conference Room A")]
    public void CreateInPerson_WithValidAddress_ShouldSucceed(string address)
    {
        // Act
        var result = MeetingLocation.CreateInPerson(address);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(LocationType.InPerson, result.Value.Type);
        Assert.Equal(address, result.Value.Details);
    }

    [Fact]
    public void CreateInPerson_WithEmptyAddress_ShouldFail()
    {
        // Act
        var result = MeetingLocation.CreateInPerson("");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void CreateCustom_WithValidDetails_ShouldSucceed()
    {
        // Arrange
        var displayName = "Skype";
        var details = "skype:username";

        // Act
        var result = MeetingLocation.CreateCustom(displayName, details);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(LocationType.Custom, result.Value.Type);
        Assert.Equal(displayName, result.Value.DisplayName);
        Assert.Equal(details, result.Value.Details);
    }

    [Fact]
    public void CreateCustom_WithEmptyDisplayName_ShouldFail()
    {
        // Act
        var result = MeetingLocation.CreateCustom("", null);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Equals_WithSameTypeAndDetails_ShouldBeEqual()
    {
        // Arrange
        var location1 = MeetingLocation.CreateZoom("https://zoom.us/j/123");
        var location2 = MeetingLocation.CreateZoom("https://zoom.us/j/123");

        // Assert
        Assert.Equal(location1, location2);
    }
}
