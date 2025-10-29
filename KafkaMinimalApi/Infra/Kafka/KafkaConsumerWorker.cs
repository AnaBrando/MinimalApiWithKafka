using KafkaMinimalApi.Domain;

namespace KafkaMinimalApi.Infra;

using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

public sealed class KafkaConsumerWorker : BackgroundService
{
    private readonly ConsumerConfig _config;
    private readonly string _topic;
    private readonly IOrderStore _store;
    private readonly IEventLog<string> _eventLog;
    private volatile bool _healthy = false;
    private ILogger<KafkaConsumerWorker> _logger;
    public bool IsHealthy => _healthy;

    public KafkaConsumerWorker(ConsumerConfig config, KafkaOptions options, IOrderStore store, IEventLog<string> eventLog,ILogger<KafkaConsumerWorker> logger)
    {
        _config = config;
        _topic = options.Topic;
        _store = store;
        _logger = logger;
        _eventLog = eventLog;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);

    private void ConsumeLoop(CancellationToken ct)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetErrorHandler((_, e) => Console.WriteLine($"[Consumer] Error: {e}"))
            .Build();

        consumer.Subscribe(_topic);
        Console.WriteLine($"[Consumer] Subscribed to {_topic}");

        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(ct);
                    _healthy = true;

                    // Deserialize and project to read model
                    var evt = JsonSerializer.Deserialize<OrderCreated>(cr.Message.Value);
                    if (evt is not null)
                    {
                        var model = new OrderReadModel(
                            evt.OrderId,
                            evt.CustomerId,
                            evt.Amount,
                            Status: "Created",
                            UpdatedAt: DateTimeOffset.UtcNow
                        );

                        _store.Upsert(model);
                        _eventLog.Add($"CONS {DateTimeOffset.UtcNow:o} off={cr.TopicPartitionOffset} key={cr.Message.Key}");
                    }

                    consumer.Commit(cr); // only after success
                }
                catch (ConsumeException ex)
                {
                    _healthy = false;
                    _eventLog.Add($"ERR {DateTimeOffset.UtcNow:o} {ex.Error.Reason}");
                    _logger.LogError(ex,"Something is Happen please check the logs");
                }
                catch (OperationCanceledException ex ) {  _logger.LogError(ex,"Something is Happen please check the logs"); }
            }
        }
        finally
        {
            try { consumer.Close(); } catch { }
        }
    }
}