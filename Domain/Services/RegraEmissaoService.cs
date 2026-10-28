using Domain.Exceptions;

namespace Domain.Services;

public sealed record DecisaoEmissao(bool DeveEmitir, int Quantidade, decimal LimitePorCartao);

public static class RegraEmissaoService
{
    public static DecisaoEmissao Avaliar(string status, int quantidadeCartoes, decimal limitePorCartao)
    {
        if (string.Equals(status, "Negada", StringComparison.OrdinalIgnoreCase)
            || quantidadeCartoes <= 0)
        {
            return new DecisaoEmissao(false, 0, 0m);
        }

        if (!string.Equals(status, "Aprovada", StringComparison.OrdinalIgnoreCase))
            throw new DomainException($"Status de proposta não suportado para emissão: {status}.");

        if (quantidadeCartoes is < 1 or > 2)
            throw new DomainException($"Quantidade de cartões inválida: {quantidadeCartoes}.");

        if (limitePorCartao <= 0)
            throw new DomainException($"Limite por cartão inválido: {limitePorCartao}.");

        return new DecisaoEmissao(true, quantidadeCartoes, limitePorCartao);
    }
}
