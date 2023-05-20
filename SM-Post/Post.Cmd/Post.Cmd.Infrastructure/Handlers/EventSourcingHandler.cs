using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStoreService _eventStoreService;

    public EventSourcingHandler(IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }
    
    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStoreService.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
        
        aggregate.MarkChangesAsCommitted();
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStoreService.GetEventsAsync(aggregateId);

        if (events is null || !events.Any())
            return aggregate;

        var latestVersion = events.Select(x => x.Version).Max();
        aggregate.Version = latestVersion;

        return aggregate;
    }
}