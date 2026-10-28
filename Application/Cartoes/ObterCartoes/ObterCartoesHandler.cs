using Application.Interfaces;

namespace Application.Cartoes.ObterCartoes;

public sealed class ObterCartoesHandler
{
    private readonly ICartaoRepository _cartoes;

    public ObterCartoesHandler(ICartaoRepository cartoes)
    {
        _cartoes = cartoes;
    }

    public async Task<IReadOnlyList<CartaoResponse>> HandleAsync(
        Guid clienteId,
        CancellationToken cancellationToken = default)
    {
        var list = await _cartoes.ListByClienteIdAsync(clienteId, cancellationToken);
        return list
            .OrderBy(c => c.Sequencia)
            .Select(c => new CartaoResponse(
                c.Id,
                c.ClienteId,
                c.PropostaId,
                c.Sequencia,
                c.NumeroMascarado,
                c.UltimosQuatro,
                c.Limite,
                c.Status.ToString(),
                c.EmitidoEmUtc))
            .ToList();
    }
}
