using Domain.Exceptions;
using Domain.Services;
using FluentAssertions;

namespace EmissaoCartao.Tests.Domain;

public class RegraEmissaoServiceTests
{
    [Fact]
    public void Avaliar_QuandoNegada_NaoEmite()
    {
        // Arrange & Act
        var result = RegraEmissaoService.Avaliar("Negada", 0, 0m);

        // Assert
        result.DeveEmitir.Should().BeFalse();
        result.Quantidade.Should().Be(0);
    }

    [Fact]
    public void Avaliar_QuandoQuantidadeZero_NaoEmite()
    {
        // Arrange & Act
        var result = RegraEmissaoService.Avaliar("Aprovada", 0, 1000m);

        // Assert
        result.DeveEmitir.Should().BeFalse();
    }

    [Fact]
    public void Avaliar_QuandoAprovadaComUmCartao_Emite()
    {
        // Arrange & Act
        var result = RegraEmissaoService.Avaliar("Aprovada", 1, 1000m);

        // Assert
        result.DeveEmitir.Should().BeTrue();
        result.Quantidade.Should().Be(1);
        result.LimitePorCartao.Should().Be(1000m);
    }

    [Fact]
    public void Avaliar_QuandoAprovadaComDoisCartoes_Emite()
    {
        // Arrange & Act
        var result = RegraEmissaoService.Avaliar("Aprovada", 2, 5000m);

        // Assert
        result.DeveEmitir.Should().BeTrue();
        result.Quantidade.Should().Be(2);
        result.LimitePorCartao.Should().Be(5000m);
    }

    [Fact]
    public void Avaliar_QuandoStatusInvalido_Lanca()
    {
        // Arrange & Act
        var act = () => RegraEmissaoService.Avaliar("Pendente", 1, 1000m);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Avaliar_QuandoQuantidadeAcimaDoPermitido_Lanca()
    {
        // Arrange & Act
        var act = () => RegraEmissaoService.Avaliar("Aprovada", 3, 1000m);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Avaliar_QuandoLimiteInvalido_Lanca()
    {
        // Arrange & Act
        var act = () => RegraEmissaoService.Avaliar("Aprovada", 1, 0m);

        // Assert
        act.Should().Throw<DomainException>();
    }
}
