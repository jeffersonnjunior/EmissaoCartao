using System.Text.Json;
using Application.Events;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Cartoes.EmitirCartoes;

public sealed class EmitirCartoesReaction : IEmitirCartoesReaction
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ICartaoRepository _cartoes;
    private readonly IOutboxWriter _outbox;

    public EmitirCartoesReaction(ICartaoRepository cartoes, IOutboxWriter outbox)
    {
        _cartoes = cartoes;
        _outbox = outbox;
    }

    public async Task ExecuteAsync(
        Guid clienteId,
        Guid propostaId,
        IReadOnlyList<Cartao> cartoes,
        CancellationToken cancellationToken = default)
    {
        await _cartoes.AddRangeAsync(cartoes, cancellationToken);

        var outboxEventId = Guid.NewGuid();
        var @event = new CartoesEmitidosEvent(
            EventId: outboxEventId,
            EventType: "CartoesEmitidos",
            SchemaVersion: 1,
            OccurredAt: DateTime.UtcNow,
            ClienteId: clienteId,
            PropostaId: propostaId,
            Cartoes: cartoes.Select(c => new CartaoEmitidoItem(
                c.Id,
                c.Sequencia,
                c.NumeroMascarado,
                c.UltimosQuatro,
                c.Limite,
                c.Status.ToString())).ToList());

        var payloadJson = JsonSerializer.Serialize(@event, JsonOptions);
        await _outbox.EnqueueAsync("CartoesEmitidos", payloadJson, outboxEventId, cancellationToken);
    }
}
