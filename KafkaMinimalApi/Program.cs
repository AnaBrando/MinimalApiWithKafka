using System.Text.Json;
using Confluent.Kafka;
using KafkaMinimalApi.Domain;
using KafkaMinimalApi.Infra;

var builder = WebApplication.CreateBuilder(args);

// 1) Carrega opções
var kafkaOptions = builder.Configuration.GetSection("Kafka").Get<KafkaOptions>() ?? new KafkaOptions();

// 2) Registra serviços (tudo ANTES de Build)
builder.Services.AddSingleton(kafkaOptions);

// Producer/Consumer configs (thread-safe)
builder.Services.AddSingleton(new ProducerConfig(KafkaConfigBuilder.BuildProducer(kafkaOptions)));
builder.Services.AddSingleton(new ConsumerConfig(KafkaConfigBuilder.BuildConsumer(kafkaOptions)));

// In-memory stores (thread-safe)
builder.Services.AddSingleton<IOrderStore, InMemoryOrderStore>();
builder.Services.AddSingleton<IEventLog<string>>(_ => new BoundedConcurrentQueueEventLog<string>(capacity: 500));

// Kafka producer
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

// (Opcional) Background consumer como Hosted Service — se você tiver essa classe
builder.Services.AddHostedService<KafkaConsumerWorker>();

var app = builder.Build();

// ---- Endpoints ----

// Health
app.MapGet("/health/live", () => Results.Ok("alive"));

// POST único para publicar um order-created (injeta também o log)
app.MapPost("/events/orders", async (OrderCreated evt, IKafkaProducer producer, IEventLog<string> log, CancellationToken ct) =>
{
    var key = evt.OrderId.ToString();
    var value = JsonSerializer.Serialize(evt);

    await producer.ProduceAsync(key, value, ct);
    log.Add($"PUB {DateTimeOffset.UtcNow:o} key={key} payload={value}");

    return Results.Accepted($"/orders/{key}");
});

// Peek do event log
app.MapGet("/_events", (IEventLog<string> log) => Results.Ok(log.Snapshot()));

app.Run();

public record OrderCreated(Guid OrderId, string CustomerId, decimal Amount);