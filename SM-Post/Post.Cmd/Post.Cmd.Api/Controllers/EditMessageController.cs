using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands.Messages;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditMessageController : ControllerBase
{
    private readonly ILogger<EditMessageController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public EditMessageController(
        ILogger<EditMessageController> logger,
        ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditMessageAsync(Guid id, EditMessageCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponse
            {
                Message = "Message edited successfully"
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
            _logger.LogError(exception, "Error editing message of a post");

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = "Error editing message of a post"
            });
        }
    }
}