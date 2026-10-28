using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class CartaoRepository : ICartaoRepository
{
    private readonly EmissaoDbContext _db;

    public CartaoRepository(EmissaoDbContext db)
    {
        _db = db;
    }

    public async Task AddRangeAsync(IEnumerable<Cartao> cartoes, CancellationToken cancellationToken = default)
    {
        await _db.Cartoes.AddRangeAsync(cartoes, cancellationToken);
    }

    public Task<bool> ExistsByPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        return _db.Cartoes.AnyAsync(x => x.PropostaId == propostaId, cancellationToken);
    }

    public async Task<IReadOnlyList<Cartao>> ListByClienteIdAsync(
        Guid clienteId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Cartoes
            .AsNoTracking()
            .Where(x => x.ClienteId == clienteId)
            .OrderBy(x => x.Sequencia)
            .ToListAsync(cancellationToken);
    }
}
