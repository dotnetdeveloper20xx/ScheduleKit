using MediatR;
using Moq;
using ScheduleKit.Application.Commands.EventTypes;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Application.Tests.Handlers;

public class CreateEventTypeCommandHandlerTests
{
    private readonly Mock<IEventTypeRepository> _eventTypeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateEventTypeCommandHandler _handler;

    public CreateEventTypeCommandHandlerTests()
    {
        _eventTypeRepositoryMock = new Mock<IEventTypeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateEventTypeCommandHandler(
            _eventTypeRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateEventType()
    {
        // Arrange
        var command = new CreateEventTypeCommand
        {
            HostUserId = Guid.NewGuid(),
            Name = "30 Minute Meeting",
            DurationMinutes = 30,
            LocationType = "Zoom",
            BufferBeforeMinutes = 5,
            BufferAfterMinutes = 10
        };

        _eventTypeRepositoryMock
            .Setup(x => x.SlugExistsAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _eventTypeRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<EventType>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(command.DurationMinutes, result.Value.DurationMinutes);

        _eventTypeRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<EventType>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSlugExists_ShouldGenerateUniqueSlug()
    {
        // Arrange
        var command = new CreateEventTypeCommand
        {
            HostUserId = Guid.NewGuid(),
            Name = "Meeting",
            DurationMinutes = 30,
            LocationType = "Zoom"
        };

        var callCount = 0;
        _eventTypeRepositoryMock
            .Setup(x => x.SlugExistsAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount <= 2; // First 2 calls return true, then false
            });

        _eventTypeRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<EventType>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("-2", result.Value.Slug); // Should have suffix -2
    }

    [Fact]
    public async Task Handle_WithInvalidLocationType_ShouldFail()
    {
        // Arrange
        var command = new CreateEventTypeCommand
        {
            HostUserId = Guid.NewGuid(),
            Name = "Meeting",
            DurationMinutes = 30,
            LocationType = "InvalidType"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Invalid location type", result.Error);
    }
}
