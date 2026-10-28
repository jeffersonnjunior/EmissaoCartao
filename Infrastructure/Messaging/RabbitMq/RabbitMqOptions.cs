namespace Infrastructure.Messaging.RabbitMq;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "credito";
    public string Password { get; set; } = "credito123";

    /// <summary>Exchange de onde consome PropostaGerada (mesmo da Proposta).</summary>
    public string ConsumeExchange { get; set; } = "credito.propostas";
    public string ConsumeRoutingKey { get; set; } = "proposta.gerada";
    public string ConsumeQueue { get; set; } = "cartao.proposta-gerada";
    public string ConsumeDlq { get; set; } = "cartao.proposta-gerada.dlq";

    /// <summary>Exchange para publicar CartoesEmitidos.</summary>
    public string PublishExchange { get; set; } = "credito.cartoes";
    public string PublishRoutingKey { get; set; } = "cartoes.emitidos";

    public bool Enabled { get; set; } = true;
}
