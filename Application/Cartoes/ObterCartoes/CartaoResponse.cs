namespace Application.Cartoes.ObterCartoes;

public sealed record CartaoResponse(
    Guid Id,
    Guid ClienteId,
    Guid PropostaId,
    int Sequencia,
    string NumeroMascarado,
    string UltimosQuatro,
    decimal Limite,
    string Status,
    DateTime EmitidoEmUtc);
