namespace Application.Events;

public sealed record CartoesEmitidosEvent(
    Guid EventId,
    string EventType,
    int SchemaVersion,
    DateTime OccurredAt,
    Guid ClienteId,
    Guid PropostaId,
    IReadOnlyList<CartaoEmitidoItem> Cartoes);

public sealed record CartaoEmitidoItem(
    Guid CartaoId,
    int Sequencia,
    string NumeroMascarado,
    string UltimosQuatro,
    decimal Limite,
    string Status);
