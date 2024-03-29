﻿using Microsoft.Extensions.Logging;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<EventHandler> _logger;

    public EventHandler(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        ILogger<EventHandler> logger)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task On(PostCreatedEvent @event)
    {
        _logger.LogInformation("Handling event {Event}", @event.GetType().Name);
        
        var post = new PostEntity
        {
            PostId = @event.Id,
            Author = @event.Author,
            DatePosted = @event.DatePosted,
            Message = @event.Message
        };

        await _postRepository.CreateAsync(post);
        
        _logger.LogInformation("Event {Event} handled successfully", @event.GetType().Name);
    }

    public async Task On(PostLikedEvent @event)
    {
        var post = await _postRepository.GetByIdAsync(@event.Id);

        if (post is null)
            return;

        post.Likes++;
        await _postRepository.UpdateAsync(post);
    }

    public async Task On(PostRemovedEvent @event)
    {
        await _postRepository.DeleteAsync(@event.Id);
    }

    public async Task On(MessageUpdatedEvent @event)
    {
        var post = await _postRepository.GetByIdAsync(@event.Id);

        if (post is null)
            return;

        post.Message = @event.Message;
        await _postRepository.UpdateAsync(post);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity()
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            CommentDate = @event.CommentDate,
            Comment = @event.Comment,
            Username = @event.Username,
            Edited = false
        };

        await _commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        var comment = await _commentRepository.GetByIdAsync(@event.CommentId);

        if (comment is null)
            return;

        comment.Comment = @event.Comment;
        comment.Edited = true;
        comment.CommentDate = @event.EditDate;

        await _commentRepository.UpdateAsync(comment);
    }

    public async Task On(CommentRemovedEvent @event)
    {
        await _commentRepository.DeleteAsync(@event.CommentId);
    }
}