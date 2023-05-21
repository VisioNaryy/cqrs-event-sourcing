﻿using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands.Posts;

public class DeletePostCommand : BaseCommand
{
    public string AuthorName { get; set; }
}