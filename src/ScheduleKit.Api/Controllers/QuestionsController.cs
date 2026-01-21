using Microsoft.AspNetCore.Mvc;
using ScheduleKit.Api.Models;
using ScheduleKit.Application.Commands.Questions;
using ScheduleKit.Application.Common.DTOs;

namespace ScheduleKit.Api.Controllers;

/// <summary>
/// API endpoints for managing booking questions on event types.
/// </summary>
[Route("api/event-types/{eventTypeId}/questions")]
public class QuestionsController : ApiControllerBase
{
    /// <summary>
    /// Add a new booking question to an event type.
    /// </summary>
    /// <param name="eventTypeId">The event type ID.</param>
    /// <param name="request">The question details.</param>
    /// <returns>The created question.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingQuestionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddQuestion(Guid eventTypeId, [FromBody] AddBookingQuestionRequest request)
    {
        var command = new AddBookingQuestionCommand
        {
            EventTypeId = eventTypeId,
            HostUserId = GetCurrentUserId(),
            QuestionText = request.QuestionText,
            Type = request.Type,
            IsRequired = request.IsRequired,
            Options = request.Options
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure && result.Error.Contains("not found"))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        if (result.IsSuccess)
        {
            return Created($"/api/event-types/{eventTypeId}/questions/{result.Value.Id}", result.Value);
        }

        return ToActionResult(result);
    }

    /// <summary>
    /// Update an existing booking question.
    /// </summary>
    /// <param name="eventTypeId">The event type ID.</param>
    /// <param name="questionId">The question ID.</param>
    /// <param name="request">The updated question details.</param>
    /// <returns>The updated question.</returns>
    [HttpPut("{questionId:guid}")]
    [ProducesResponseType(typeof(BookingQuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuestion(
        Guid eventTypeId,
        Guid questionId,
        [FromBody] UpdateBookingQuestionRequest request)
    {
        var command = new UpdateBookingQuestionCommand
        {
            EventTypeId = eventTypeId,
            QuestionId = questionId,
            HostUserId = GetCurrentUserId(),
            QuestionText = request.QuestionText,
            Type = request.Type,
            IsRequired = request.IsRequired,
            Options = request.Options
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure && result.Error.Contains("not found"))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return ToActionResult(result);
    }

    /// <summary>
    /// Delete a booking question.
    /// </summary>
    /// <param name="eventTypeId">The event type ID.</param>
    /// <param name="questionId">The question ID.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{questionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion(Guid eventTypeId, Guid questionId)
    {
        var command = new DeleteBookingQuestionCommand
        {
            EventTypeId = eventTypeId,
            QuestionId = questionId,
            HostUserId = GetCurrentUserId()
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure && result.Error.Contains("not found"))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return ToActionResult(result);
    }

    /// <summary>
    /// Reorder booking questions.
    /// </summary>
    /// <param name="eventTypeId">The event type ID.</param>
    /// <param name="request">The new order of question IDs.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderQuestions(Guid eventTypeId, [FromBody] ReorderQuestionsRequest request)
    {
        var command = new ReorderBookingQuestionsCommand
        {
            EventTypeId = eventTypeId,
            HostUserId = GetCurrentUserId(),
            QuestionIds = request.QuestionIds
        };

        var result = await Mediator.Send(command);

        if (result.IsFailure && result.Error.Contains("not found"))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return ToActionResult(result);
    }
}
