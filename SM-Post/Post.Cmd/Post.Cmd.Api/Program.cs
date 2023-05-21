using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Configs;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Post.Cmd.Api;
using Post.Cmd.Infrastructure.Producers;
using Post.Common.Events;

var builder = WebApplication.CreateBuilder(args);

BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();
BsonClassMap.RegisterClassMap<PostLikedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentRemovedEvent>();

// Add services to the container.

var services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<MongoDbConfig>(builder.Configuration.GetSection($"{nameof(MongoDbConfig)}"));
services.Configure<ProducerConfig>(builder.Configuration.GetSection($"{nameof(ProducerConfig)}"));
services.AddScoped<IMongoEventStoreRepository, MongoMongoEventStoreRepository>();
services.AddScoped<IEventStoreService, EventStoreService>();
services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
services.AddScoped<ICommandHandler, CommandHandler>();
services.AddScoped<IKafkaEventProducer, KafkaKafkaEventProducer>();

// Register command handler methods
services.AddCommands();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
