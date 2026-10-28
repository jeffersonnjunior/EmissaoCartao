using Domain.Entities;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public sealed class EmissaoDbContext : DbContext
{
    public EmissaoDbContext(DbContextOptions<EmissaoDbContext> options) : base(options)
    {
    }

    public DbSet<Cartao> Cartoes => Set<Cartao>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmissaoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
