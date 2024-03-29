﻿using Post.Cmd.Api.Commands.Comments;
using Post.Cmd.Api.Commands.Db;
using Post.Cmd.Api.Commands.Messages;
using Post.Cmd.Api.Commands.Posts;

namespace Post.Cmd.Api.Commands;

public interface ICommandHandler
{
    Task HandleAsync(AddCommentCommand command);
    Task HandleAsync(DeletePostCommand command);
    Task HandleAsync(EditCommentCommand command);
    Task HandleAsync(EditMessageCommand command);
    Task HandleAsync(LikePostCommand command);
    Task HandleAsync(NewPostCommand command);
    Task HandleAsync(RemoveCommentCommand command);
    Task HandleAsync(RestoreReadDbCommand command);
}