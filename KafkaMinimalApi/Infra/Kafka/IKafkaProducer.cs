namespace KafkaMinimalApi.Infra;

public interface IKafkaProducer
{
    Task ProduceAsync(string key, string value, CancellationToken ct = default);
}