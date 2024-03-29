﻿using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class KafkaEventConsumer : IEventConsumer
{
    private readonly IEventHandler _eventHandler;
    private readonly ILogger<KafkaEventConsumer> _logger;
    private readonly ConsumerConfig _config;

    public KafkaEventConsumer(
        IOptions<ConsumerConfig> config, 
        IEventHandler eventHandler,
        ILogger<KafkaEventConsumer> logger)
    {
        _eventHandler = eventHandler;
        _logger = logger;
        _config = config.Value;
    }

    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
        
        consumer.Subscribe(topic);

        while (true)
        {
            var consumerResult = consumer.Consume();
            
            _logger.LogInformation("Consuming message from topic {Topic}", topic);
            
            if (consumerResult?.Message is null)
                continue;

            var options = new JsonSerializerOptions {Converters = { new EventJsonConverter() }};
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);
            var handlerMethod = _eventHandler.GetType().GetMethod("On", new [] {@event.GetType()});

            if (handlerMethod is null)
                throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");

            handlerMethod.Invoke(_eventHandler, new object?[] {@event});
            consumer.Commit(consumerResult);
            
            _logger.LogInformation("Message consumed from topic {Topic}", topic);
        }
    }
}