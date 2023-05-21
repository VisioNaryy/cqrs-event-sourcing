using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands.Posts;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LikePostController : ControllerBase
{
    private readonly ILogger<LikePostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public LikePostController(
        ILogger<LikePostController> logger,
        ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> LikePostAsync(Guid id)
    {
        try
        {
            await _commandDispatcher.SendAsync(new LikePostCommand {Id = id});

            return Ok(new BaseResponse
            {
                Message = "Post liked successfully"
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
            _logger.LogError(exception, "Error liking a post");

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = "Error liking a post"
            });
        }
    }
}