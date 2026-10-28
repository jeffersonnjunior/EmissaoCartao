using Domain.Entities;

namespace Application.Cartoes.EmitirCartoes;

/// <summary>
/// Side-effects da emissão (persistência + outbox).
/// Orquestração/idempotência ficam no Handler.
/// </summary>
public interface IEmitirCartoesReaction
{
    Task ExecuteAsync(
        Guid clienteId,
        Guid propostaId,
        IReadOnlyList<Cartao> cartoes,
        CancellationToken cancellationToken = default);
}
