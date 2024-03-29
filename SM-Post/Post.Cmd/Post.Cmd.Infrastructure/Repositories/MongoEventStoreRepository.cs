﻿using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Configs;

namespace Post.Cmd.Infrastructure.Repositories;

public class MongoEventStoreRepository : IMongoEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    public MongoEventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        =>
            await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync()
                .ConfigureAwait(false);

    public async Task<List<EventModel>> FindAll()
    {
        var eventStream = await _eventStoreCollection.FindAsync(x => true).ConfigureAwait(false);

        return await eventStream.ToListAsync().ConfigureAwait(false);
    }


    public async Task SaveAsync(EventModel @event)
        =>
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
}