﻿using CQRS.Core.Infrastructure;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.Commands.Comments;
using Post.Cmd.Api.Commands.Db;
using Post.Cmd.Api.Commands.Messages;
using Post.Cmd.Api.Commands.Posts;
using Post.Cmd.Infrastructure.Dispatchers;

namespace Post.Cmd.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler, CommandHandler>();
        
        var commandHandler = services.BuildServiceProvider().GetRequiredService<ICommandHandler>();

        var dispatcher = new CommandDispatcher();

        dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<RestoreReadDbCommand>(commandHandler.HandleAsync);
        services.AddSingleton<ICommandDispatcher>(_ => dispatcher);

        return services;
    }
}