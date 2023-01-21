using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Configs;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;
using Microsoft.Extensions.DependencyInjection;
using Post.Cmd.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<MongoDbConfig>(builder.Configuration.GetSection($"{nameof(MongoDbConfig)}"));
services.AddScoped<IEventStoreRepository, EventStoreRepository>();
services.AddScoped<IEventStore, EventStore>();
services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
services.AddScoped<ICommandHandler, CommandHandler>();

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
