﻿using CQRS.Core.Exceptions;
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

        command.Id = id;

        await _commandDispatcher.SendAsync(command);

        return StatusCode(StatusCodes.Status201Created, new NewPostResponse
        {
            Message = "New post created successfully",
            Id = id
        });
    }
}