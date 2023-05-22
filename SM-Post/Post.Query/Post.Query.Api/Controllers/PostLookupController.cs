using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostLookupController : ControllerBase
{
    private readonly ILogger<PostLookupController> _logger;
    private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

    public PostLookupController(
        ILogger<PostLookupController> logger,
        IQueryDispatcher<PostEntity> queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPostsAsync()
    {
        var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());
        
        if (posts is null || !posts.Any())
            return NoContent();

        return Ok(new PostLookupResponse
        {
            Message = $"Successfully returned {posts.Count} posts",
            Posts = posts
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostByIdAsync(Guid id)
    {
        var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery
        {
            Id = id
        });

        if (posts is null || !posts.Any())
            return NoContent();

        return Ok(new PostLookupResponse
        {
            Message = "Successfully returned post",
            Posts = posts
        });
    }
    
    [HttpGet("author/{author}")]
    public async Task<IActionResult> GetPostsByAuthorAsync(string author)
    {
        var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery
        {
            Author = author
        });
        
        if (posts is null || !posts.Any())
            return NoContent();

        return Ok(new PostLookupResponse
        {
            Message = $"Successfully returned {posts.Count} posts",
            Posts = posts
        });
    }

    [HttpGet("message")]
    public async Task<IActionResult> GetPostsByMessageAsync()
    {
        var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());

        if (posts is null || !posts.Any())
            return NoContent();

        return Ok(new PostLookupResponse
        {
            Message = $"Successfully returned {posts.Count} posts",
            Posts = posts
        });
    }
    
    [HttpGet("likes/{numberOfLikes}")]
    public async Task<IActionResult> GetPostsWithLikesAsync(int numberOfLikes)
    {
        var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery
        {
            NumberOfLikes = numberOfLikes
        });

        if (posts is null || !posts.Any())
            return NoContent();

        return Ok(new PostLookupResponse
        {
            Message = $"Successfully returned {posts.Count} posts",
            Posts = posts
        });
    }
}