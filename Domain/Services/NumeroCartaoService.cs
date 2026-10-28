using System.Security.Cryptography;
using System.Text;

namespace Domain.Services;

/// <summary>
/// Gera PAN sintético determinístico (não é cartão real / PCI).
/// </summary>
public static class NumeroCartaoService
{
    public static (string NumeroCompleto, string UltimosQuatro, string Mascarado) Gerar(
        Guid propostaId,
        int sequencia)
    {
        var seed = $"{propostaId:N}:{sequencia}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(seed));
        var sb = new StringBuilder(16);
        sb.Append("4000");
        for (var i = 0; i < 12; i++)
            sb.Append((hash[i] % 10).ToString());

        var numero = sb.ToString();
        var ultimos = numero[^4..];
        var mascarado = $"**** **** **** {ultimos}";
        return (numero, ultimos, mascarado);
    }
}
