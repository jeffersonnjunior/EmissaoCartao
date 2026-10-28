using Domain.Entities;

namespace Application.Interfaces;

public interface ICartaoRepository
{
    Task AddRangeAsync(IEnumerable<Cartao> cartoes, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cartao>> ListByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default);
}
