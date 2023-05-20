using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands.Messages;

public class EditMessageCommand : BaseCommand
{
    public string Message { get; set; }
}