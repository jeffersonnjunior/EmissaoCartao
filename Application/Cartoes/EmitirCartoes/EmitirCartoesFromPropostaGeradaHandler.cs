using Application.Events;
using Application.Interfaces;
using Domain.Entities;
using Domain.Services;

namespace Application.Cartoes.EmitirCartoes;

/// <summary>
/// Orquestra o job "EmitirCartoesCredito": gates de idempotência + domínio + reaction.
/// </summary>
public sealed class EmitirCartoesFromPropostaGeradaHandler
{
    public const string JobDisplayName = "EmitirCartoesCredito";

    private readonly ICartaoRepository _cartoes;
    private readonly IProcessedEventStore _processedEvents;
    private readonly IEmitirCartoesReaction _reaction;
    private readonly IUnitOfWork _unitOfWork;

    public EmitirCartoesFromPropostaGeradaHandler(
        ICartaoRepository cartoes,
        IProcessedEventStore processedEvents,
        IEmitirCartoesReaction reaction,
        IUnitOfWork unitOfWork)
    {
        _cartoes = cartoes;
        _processedEvents = processedEvents;
        _reaction = reaction;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        PropostaGeradaEvent @event,
        CancellationToken cancellationToken = default)
    {
        if (await _processedEvents.ExistsAsync(@event.EventId, cancellationToken))
            return;

        if (await _cartoes.ExistsByPropostaIdAsync(@event.Payload.PropostaId, cancellationToken))
        {
            await _processedEvents.MarkProcessedAsync(@event.EventId, @event.EventType, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var decisao = RegraEmissaoService.Avaliar(
            @event.Payload.Status,
            @event.Payload.QuantidadeCartoes,
            @event.Payload.LimitePorCartao);

        if (!decisao.DeveEmitir)
        {
            await _processedEvents.MarkProcessedAsync(@event.EventId, @event.EventType, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var cartoes = new List<Cartao>(decisao.Quantidade);
        for (var i = 1; i <= decisao.Quantidade; i++)
        {
            cartoes.Add(Cartao.Emitir(
                @event.ClienteId,
                @event.Payload.PropostaId,
                sequencia: i,
                limite: decisao.LimitePorCartao));
        }

        await _reaction.ExecuteAsync(
            @event.ClienteId,
            @event.Payload.PropostaId,
            cartoes,
            cancellationToken);

        await _processedEvents.MarkProcessedAsync(@event.EventId, @event.EventType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
