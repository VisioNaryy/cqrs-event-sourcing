using System.Net;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands.Posts;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewPostController : ControllerBase
{
    private readonly ILogger<NewPostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public NewPostController(
        ILogger<NewPostController> logger,
        ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }
    
    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
    {
        var id = Guid.NewGuid();
        
        try
        {
            command.Id = id;
        
            await _commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status201Created, new NewPostResponse
            {
                Message = "New post created successfully",
                Id = id
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error creating new post");

            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
            {
                Message = "Error creating new post",
                Id = id
            });
        }
    }
}