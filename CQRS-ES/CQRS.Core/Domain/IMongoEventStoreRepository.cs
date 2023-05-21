using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface IMongoEventStoreRepository
{
    Task SaveAsync(EventModel @event);

    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
}