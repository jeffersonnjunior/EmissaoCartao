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
        // Arrange
        var clienteId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();

        // Act
        var cartao = Cartao.Emitir(clienteId, propostaId, sequencia: 1, limite: 1000m);

        // Assert
        cartao.Status.Should().Be(StatusCartao.Emitido);
        cartao.UltimosQuatro.Should().HaveLength(4);
        cartao.NumeroMascarado.Should().StartWith("****");
        cartao.Limite.Should().Be(1000m);
        cartao.Sequencia.Should().Be(1);
        cartao.ClienteId.Should().Be(clienteId);
        cartao.PropostaId.Should().Be(propostaId);
    }

    [Fact]
    public void Emitir_MesmaPropostaESequencia_DeveSerDeterministico()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();

        // Act
        var a = Cartao.Emitir(clienteId, propostaId, 1, 5000m);
        var b = Cartao.Emitir(clienteId, propostaId, 1, 5000m);

        // Assert
        a.UltimosQuatro.Should().Be(b.UltimosQuatro);
        a.NumeroMascarado.Should().Be(b.NumeroMascarado);
    }

    [Fact]
    public void Emitir_QuandoLimiteInvalido_Lanca()
    {
        // Arrange & Act
        var act = () => Cartao.Emitir(Guid.NewGuid(), Guid.NewGuid(), 1, 0m);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Emitir_QuandoSequenciaInvalida_Lanca()
    {
        // Arrange & Act
        var act = () => Cartao.Emitir(Guid.NewGuid(), Guid.NewGuid(), 0, 1000m);

        // Assert
        act.Should().Throw<DomainException>();
    }
}
