namespace KafkaMinimalApi.Infra;

public sealed class KafkaProducer : IKafkaProducer, IAsyncDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducer(ProducerConfig config, KafkaOptions options)
    {
        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => Console.WriteLine($"[Producer] Error: {e}"))
            .Build();
        _topic = options.Topic;
    }

    public async Task ProduceAsync(string key, string value, CancellationToken ct = default)
    {
        try
        {
            var msg = new Message<string, string> { Key = key, Value = value };
            var delivery = await _producer.ProduceAsync(_topic, msg, ct);
        }
        catch (Exception e)
        {
            
            Console.WriteLine(e);
            throw;
        }
      
    }

    public async ValueTask DisposeAsync()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
        await Task.CompletedTask;
    }
}