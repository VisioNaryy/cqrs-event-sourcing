using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Configs;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;
using MongoDB.Bson.Serialization;
using Post.Cmd.Api;
using Post.Cmd.Api.Middleware;
using Post.Cmd.Infrastructure.Producers;
using Post.Common.Events;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Configuration.AddUserSecrets<Program>();

BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();
BsonClassMap.RegisterClassMap<PostLikedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentRemovedEvent>();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<ExceptionMiddleware>();
services.Configure<MongoDbConfig>(builder.Configuration.GetSection($"{nameof(MongoDbConfig)}"));
services.Configure<ProducerConfig>(builder.Configuration.GetSection($"{nameof(ProducerConfig)}"));
services.AddScoped<IMongoEventStoreRepository, MongoEventStoreRepository>();
services.AddScoped<IEventStoreService, EventStoreService>();
services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
services.AddScoped<IKafkaEventProducer, KafkaKafkaEventProducer>();

// Register command handler methods
services.AddCommands();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
