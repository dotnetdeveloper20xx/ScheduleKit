using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScheduleKit.Api.Controllers;
using ScheduleKit.Api.Models;
using ScheduleKit.Application.Commands.EventTypes;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Queries.EventTypes;
using ScheduleKit.Domain.Common;

namespace ScheduleKit.Api.Tests.Controllers;

public class EventTypesControllerTests
{
    private readonly Mock<ISender> _mediatorMock;
    private readonly EventTypesController _controller;

    public EventTypesControllerTests()
    {
        _mediatorMock = new Mock<ISender>();
        _controller = new EventTypesController();

        // Setup HttpContext with mock mediator
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = CreateServiceProviderWithMediator();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private IServiceProvider CreateServiceProviderWithMediator()
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(ISender)))
            .Returns(_mediatorMock.Object);
        return serviceProviderMock.Object;
    }

    [Fact]
    public async Task GetEventTypes_ShouldReturnOkWithEventTypes()
    {
        // Arrange
        var eventTypes = new List<EventTypeResponse>
        {
            new EventTypeResponse
            {
                Id = Guid.NewGuid(),
                Name = "30 Minute Meeting",
                Slug = "30-minute-meeting",
                DurationMinutes = 30,
                LocationType = "Zoom"
            }
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetEventTypesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(eventTypes));

        // Act
        var result = await _controller.GetEventTypes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEventTypes = Assert.IsType<List<EventTypeResponse>>(okResult.Value);
        Assert.Single(returnedEventTypes);
    }

    [Fact]
    public async Task GetEventType_WhenFound_ShouldReturnOk()
    {
        // Arrange
        var eventTypeId = Guid.NewGuid();
        var eventType = new EventTypeResponse
        {
            Id = eventTypeId,
            Name = "Meeting",
            Slug = "meeting",
            DurationMinutes = 30,
            LocationType = "Zoom"
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetEventTypeByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(eventType));

        // Act
        var result = await _controller.GetEventType(eventTypeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEventType = Assert.IsType<EventTypeResponse>(okResult.Value);
        Assert.Equal(eventTypeId, returnedEventType.Id);
    }

    [Fact]
    public async Task GetEventType_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var eventTypeId = Guid.NewGuid();

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetEventTypeByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<EventTypeResponse>("Event type not found."));

        // Act
        var result = await _controller.GetEventType(eventTypeId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
        Assert.Contains("not found", problemDetails.Detail);
    }

    [Fact]
    public async Task CreateEventType_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateEventTypeRequest
        {
            Name = "New Meeting",
            DurationMinutes = 30,
            LocationType = "Zoom"
        };

        var createdEventType = new EventTypeResponse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = "new-meeting",
            DurationMinutes = request.DurationMinutes,
            LocationType = request.LocationType
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateEventTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(createdEventType));

        // Act
        var result = await _controller.CreateEventType(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedEventType = Assert.IsType<EventTypeResponse>(createdResult.Value);
        Assert.Equal(request.Name, returnedEventType.Name);
    }

    [Fact]
    public async Task DeleteEventType_WhenFound_ShouldReturnNoContent()
    {
        // Arrange
        var eventTypeId = Guid.NewGuid();

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<DeleteEventTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.DeleteEventType(eventTypeId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteEventType_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var eventTypeId = Guid.NewGuid();

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<DeleteEventTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Event type not found."));

        // Act
        var result = await _controller.DeleteEventType(eventTypeId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
