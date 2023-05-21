using CQRS.Core.Events;

namespace CQRS.Core.Producers;

public interface IKafkaEventProducer
{
    Task ProduceAsync<T>(string? topic, T @event) where T: BaseEvent;
}