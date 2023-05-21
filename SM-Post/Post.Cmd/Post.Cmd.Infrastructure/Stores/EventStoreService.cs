using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStoreService : IEventStoreService
{
    private readonly IMongoEventStoreRepository _mongoEventStoreRepository;
    private readonly IKafkaEventProducer _kafkaEventProducer;

    public EventStoreService(IMongoEventStoreRepository mongoEventStoreRepository, IKafkaEventProducer kafkaEventProducer)
    {
        _mongoEventStoreRepository = mongoEventStoreRepository;
        _kafkaEventProducer = kafkaEventProducer;
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _mongoEventStoreRepository.FindByAggregateId(aggregateId);

        if (eventStream is null || !eventStream.Any())
            throw new AggregateNotFoundException("Incorrect post Id provided!");

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }
    
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _mongoEventStoreRepository.FindByAggregateId(aggregateId);

        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            throw new ConcurrencyException();

        var version = expectedVersion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                TimeStamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = @event
            };

            await _mongoEventStoreRepository.SaveAsync(eventModel);

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await _kafkaEventProducer.ProduceAsync(topic, @event);
        }
    }
}