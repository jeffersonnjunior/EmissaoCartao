using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace EmissaoCartao.Tests.Domain;

public class CartaoTests
{
    [Fact]
    public void Emitir_DeveCriarCartaoMascarado()
    {
        var clienteId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();

        var cartao = Cartao.Emitir(clienteId, propostaId, sequencia: 1, limite: 1000m);

        cartao.Status.Should().Be(StatusCartao.Emitido);
        cartao.UltimosQuatro.Should().HaveLength(4);
        cartao.NumeroMascarado.Should().StartWith("****");
        cartao.Limite.Should().Be(1000m);
        cartao.Sequencia.Should().Be(1);
    }

    [Fact]
    public void Emitir_QuandoLimiteInvalido_Lanca()
    {
        var act = () => Cartao.Emitir(Guid.NewGuid(), Guid.NewGuid(), 1, 0m);

        act.Should().Throw<DomainException>();
    }
}
