using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands.Comments;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddCommentController: ControllerBase
{
    private readonly ILogger<AddCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public AddCommentController(
        ILogger<AddCommentController> logger,
        ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> AddCommentAsync(Guid id, AddCommentCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponse
            {
                Message = "Comment added successfully"
            });
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "Client made a bad request");

            return BadRequest(new BaseResponse
            {
                Message = exception.Message
            });
        }
        catch (AggregateNotFoundException exception)
        {
            _logger.LogError(exception, "Could not find an aggregate, client passed an invalid id");

            return BadRequest(new BaseResponse
            {
                Message = exception.Message
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error adding a comment to a post");

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = "Error adding a comment to a post"
            });
        }
    }
}