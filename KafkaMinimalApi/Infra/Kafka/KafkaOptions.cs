namespace KafkaMinimalApi.Infra;

public sealed class KafkaOptions
{
    public string BootstrapServers { get; set; } = "";
    public string Topic { get; set; } = "";
    public string ConsumerGroupId { get; set; } = "minimal-api";
    public bool EnableAutoCreateTopics { get; set; } = false;

    public KafkaSecurityOptions Security { get; set; } = new();
}