using Confluent.Kafka;
using CQRS.Core.Consumers;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api;
using Post.Query.Api.Middleware;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;
using EventHandler = Post.Query.Infrastructure.Handlers.EventHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var services = builder.Services;

Action<DbContextOptionsBuilder> configureDbContext =
    options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
services.AddDbContext<DatabaseContext>(configureDbContext);
services.AddSingleton(new DatabaseContextFactory(configureDbContext));

var dataContext = services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();

services.AddScoped<ExceptionMiddleware>();
services.AddScoped<IPostRepository, PostRepository>();
services.AddScoped<ICommentRepository, CommentRepository>();
services.AddScoped<IEventHandler, EventHandler>();
services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
services.AddScoped<IEventConsumer, KafkaEventConsumer>();
services.AddHostedService<ConsumerHostedService>();

services.AddQueries();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

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