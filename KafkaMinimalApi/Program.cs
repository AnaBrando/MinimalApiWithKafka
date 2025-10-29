using System.Text.Json;
using KafkaMinimalApi.Domain;
using KafkaMinimalApi.Infra;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var kafkaOptions = builder.Configuration.GetSection("Kafka").Get<KafkaOptions>() ?? new KafkaOptions();
builder.Services.AddSingleton(kafkaOptions);

// 2) Register Kafka configs (producer + consumer configs are thread-safe)
builder.Services.AddSingleton(new ProducerConfig(KafkaConfigBuilder.BuildProducer(kafkaOptions)));
builder.Services.AddSingleton(new ConsumerConfig(KafkaConfigBuilder.BuildConsumer(kafkaOptions)));

// 3) Register your thread-safe in-memory stores
builder.Services.AddSingleton<IOrderStore, InMemoryOrderStore>();
builder.Services.AddSingleton<IEventLog<string>>(_ => new BoundedConcurrentQueueEventLog<string>(capacity: 500));

// 4) Register Kafka producer
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

// 5) Register background Kafka consumer as Hosted Service
app.MapPost("/events/orders", async (OrderCreated evt, IKafkaProducer producer, CancellationToken ct) =>
{
    // normally you'd validate & serialize to JSON; here we send a simple string
    var key = evt.OrderId.ToString();
    var value = System.Text.Json.JsonSerializer.Serialize(evt);
    await producer.ProduceAsync(key, value, ct);
    return Results.Accepted($"/events/orders/{key}");
});

// Simple health endpoints
app.MapGet("/health/ready", (KafkaConsumerWorker consumer) =>
    consumer.IsHealthy ? Results.Ok("ready") : Results.StatusCode(503));
// POST to publish an order-created event
app.MapPost("/events/orders", async (OrderCreated evt, IKafkaProducer producer, IEventLog<string> log, CancellationToken ct) =>
{
    var key = evt.OrderId.ToString();
    var value = JsonSerializer.Serialize(evt);
    
    await producer.ProduceAsync(key, value, ct);

    log.Add($"PUB {DateTimeOffset.UtcNow:o} key={key} payload={value}");
    return Results.Accepted($"/orders/{key}");
});

// Peek recent in-memory event log
app.MapGet("/_events", (IEventLog<string> log) => Results.Ok(log.Snapshot()));

app.MapGet("/health/live", () => Results.Ok("alive"));
app.Run();

public record OrderCreated(Guid OrderId, string CustomerId, decimal Amount);

