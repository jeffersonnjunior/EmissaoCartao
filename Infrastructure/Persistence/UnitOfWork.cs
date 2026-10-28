using Application.Interfaces;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly EmissaoDbContext _db;

    public UnitOfWork(EmissaoDbContext db)
    {
        _db = db;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
