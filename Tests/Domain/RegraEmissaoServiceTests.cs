using Domain.Exceptions;
using Domain.Services;
using FluentAssertions;

namespace EmissaoCartao.Tests.Domain;

public class RegraEmissaoServiceTests
{
    [Fact]
    public void Avaliar_QuandoNegada_NaoEmite()
    {
        var result = RegraEmissaoService.Avaliar("Negada", 0, 0m);

        result.DeveEmitir.Should().BeFalse();
        result.Quantidade.Should().Be(0);
    }

    [Fact]
    public void Avaliar_QuandoAprovadaComUmCartao_Emite()
    {
        var result = RegraEmissaoService.Avaliar("Aprovada", 1, 1000m);

        result.DeveEmitir.Should().BeTrue();
        result.Quantidade.Should().Be(1);
        result.LimitePorCartao.Should().Be(1000m);
    }

    [Fact]
    public void Avaliar_QuandoAprovadaComDoisCartoes_Emite()
    {
        var result = RegraEmissaoService.Avaliar("Aprovada", 2, 5000m);

        result.DeveEmitir.Should().BeTrue();
        result.Quantidade.Should().Be(2);
        result.LimitePorCartao.Should().Be(5000m);
    }

    [Fact]
    public void Avaliar_QuandoStatusInvalido_Lanca()
    {
        var act = () => RegraEmissaoService.Avaliar("Pendente", 1, 1000m);

        act.Should().Throw<DomainException>();
    }
}
