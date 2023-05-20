using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands.Comments;

public class AddCommentCommand : BaseCommand
{
    public string Comment { get; set; }
    public string Username { get; set; }
}