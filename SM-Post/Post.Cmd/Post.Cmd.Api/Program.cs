using CQRS.Core.Domain;
using CQRS.Core.Infrastructure;
using Post.Cmd.Infrastructure.Configs;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<MongoDbConfig>(builder.Configuration.GetSection($"{nameof(MongoDbConfig)}"));
services.AddScoped<IEventStoreRepository, EventStoreRepository>();
services.AddScoped<IEventStore, EventStore>();

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
