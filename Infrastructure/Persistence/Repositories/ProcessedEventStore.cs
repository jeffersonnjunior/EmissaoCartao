using Application.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class ProcessedEventStore : IProcessedEventStore
{
    private readonly EmissaoDbContext _db;

    public ProcessedEventStore(EmissaoDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return _db.ProcessedEvents.AnyAsync(x => x.EventId == eventId, cancellationToken);
    }

    public async Task MarkProcessedAsync(
        Guid eventId,
        string eventType,
        CancellationToken cancellationToken = default)
    {
        await _db.ProcessedEvents.AddAsync(new ProcessedEvent
        {
            EventId = eventId,
            EventType = eventType,
            ProcessedAtUtc = DateTime.UtcNow
        }, cancellationToken);
    }
}
