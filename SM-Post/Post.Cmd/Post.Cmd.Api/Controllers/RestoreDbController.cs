using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands.Db;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestoreDbController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public RestoreDbController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("restore")]
    public async Task<ActionResult> RestoreDb()
    {
        var command = new RestoreReadDbCommand();
        await _commandDispatcher.SendAsync(command);

        return StatusCode(StatusCodes.Status201Created, new BaseResponse
        {
            Message = "Db restored successfully"
        });
    }
}