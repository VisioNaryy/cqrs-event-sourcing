using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands.Comments;

public class RemoveCommentCommand : BaseCommand
{
    public string Username { get; set; }
}