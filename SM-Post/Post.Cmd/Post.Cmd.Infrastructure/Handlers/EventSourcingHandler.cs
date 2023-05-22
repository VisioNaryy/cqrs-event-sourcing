using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStoreService _eventStoreService;
    private readonly IKafkaEventProducer _kafkaEventProducer;

    public EventSourcingHandler(
        IEventStoreService eventStoreService,
        IKafkaEventProducer kafkaEventProducer)
    {
        _eventStoreService = eventStoreService;
        _kafkaEventProducer = kafkaEventProducer;
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

        aggregate.ReplayEvents(events);
        var latestVersion = events.Select(x => x.Version).Max();
        aggregate.Version = latestVersion;

        return aggregate;
    }

    public async Task RepublishEventsAsync()
    {
        var aggregateIds = await _eventStoreService.GetAggregateIdsAsync();

        foreach (var aggregateId in aggregateIds)
        {
            var aggregate = await GetByIdAsync(aggregateId);
            
            if (aggregate is null || !aggregate.Active)
                continue;
            
            var events = await _eventStoreService.GetEventsAsync(aggregateId);

            foreach (var @event in events)
            {
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await _kafkaEventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}