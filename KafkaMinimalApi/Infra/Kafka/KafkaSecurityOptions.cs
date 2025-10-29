namespace KafkaMinimalApi.Infra;

public sealed class KafkaSecurityOptions
{
    public bool UseSasl { get; set; } = false;
    public string Mechanism { get; set; } = "Plain"; // Plain, ScramSha256, ScramSha512, OAuthBearer
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Protocol { get; set; } = "SaslSsl"; // SaslSsl, SaslPlaintext, Ssl, Plaintext
}