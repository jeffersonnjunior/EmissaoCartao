using Domain.Enums;
using Domain.Exceptions;
using Domain.Services;

namespace Domain.Entities;

public sealed class Cartao
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public Guid PropostaId { get; private set; }
    public int Sequencia { get; private set; }
    public string UltimosQuatro { get; private set; } = null!;
    public string NumeroMascarado { get; private set; } = null!;
    public decimal Limite { get; private set; }
    public StatusCartao Status { get; private set; }
    public DateTime EmitidoEmUtc { get; private set; }

    private Cartao()
    {
    }

    public static Cartao Emitir(
        Guid clienteId,
        Guid propostaId,
        int sequencia,
        decimal limite)
    {
        if (clienteId == Guid.Empty)
            throw new DomainException("ClienteId é obrigatório.");
        if (propostaId == Guid.Empty)
            throw new DomainException("PropostaId é obrigatório.");
        if (sequencia < 1)
            throw new DomainException("Sequência do cartão deve ser >= 1.");
        if (limite <= 0)
            throw new DomainException("Limite do cartão deve ser positivo.");

        var (_, ultimos, mascarado) = NumeroCartaoService.Gerar(propostaId, sequencia);

        return new Cartao
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            PropostaId = propostaId,
            Sequencia = sequencia,
            UltimosQuatro = ultimos,
            NumeroMascarado = mascarado,
            Limite = limite,
            Status = StatusCartao.Emitido,
            EmitidoEmUtc = DateTime.UtcNow
        };
    }
}
