using CQRS.Core.Infrastructure;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Infrastructure.Dispatchers;

namespace Post.Query.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler, QueryHandler>();
        
        var queryHandler = services.BuildServiceProvider().GetRequiredService<IQueryHandler>();

        var dispatcher = new QueryDispatcher();
        
        dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
        dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
        dispatcher.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
        dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(queryHandler.HandleAsync);
        dispatcher.RegisterHandler<FindPostsWithLikesQuery>(queryHandler.HandleAsync);
        services.AddSingleton<IQueryDispatcher<PostEntity>>(_ => dispatcher);   
        
        return services;
    }
}