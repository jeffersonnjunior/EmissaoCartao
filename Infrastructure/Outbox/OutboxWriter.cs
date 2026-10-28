using Application.Interfaces;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Outbox;

public sealed class OutboxWriter : IOutboxWriter
{
    private readonly EmissaoDbContext _db;

    public OutboxWriter(EmissaoDbContext db)
    {
        _db = db;
    }

    public async Task EnqueueAsync(
        string eventType,
        string payloadJson,
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        await _db.OutboxMessages.AddAsync(new OutboxMessage
        {
            Id = eventId,
            EventType = eventType,
            PayloadJson = payloadJson,
            OccurredAtUtc = DateTime.UtcNow,
            Attempts = 0
        }, cancellationToken);
    }
}
