namespace KafkaMinimalApi.Infra;

public static class KafkaConfigBuilder
{
    public static ProducerConfig BuildProducer(KafkaOptions opt) =>
        new ProducerConfig
            {
                BootstrapServers = opt.BootstrapServers,
                AllowAutoCreateTopics = opt.EnableAutoCreateTopics,
                Acks = Acks.All,
                LingerMs = 5,
                EnableIdempotence = true // safe, exactly-once in producer
            }
            .WithSecurity(opt);

    public static ConsumerConfig BuildConsumer(KafkaOptions opt) =>
        new ConsumerConfig
            {
                BootstrapServers = opt.BootstrapServers,
                GroupId = opt.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false, // manual commit for safety
                AllowAutoCreateTopics = opt.EnableAutoCreateTopics
            }
            .WithSecurity(opt);

    private static T WithSecurity<T>(this T config, KafkaOptions opt) where T : ClientConfig
    {
        if (opt.Security?.UseSasl == true)
        {
            config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(opt.Security.Protocol, out var proto)
                ? proto : SecurityProtocol.SaslSsl;

            config.SaslMechanism = opt.Security.Mechanism?.ToLower() switch
            {
                "scramsha256" => SaslMechanism.ScramSha256,
                "scramsha512" => SaslMechanism.ScramSha512,
                "oauthbearer" => SaslMechanism.OAuthBearer,
                _ => SaslMechanism.Plain
            };
            config.SaslUsername = opt.Security.Username;
            config.SaslPassword = opt.Security.Password;
        }
        return config;
    }
}