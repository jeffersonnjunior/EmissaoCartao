using Application.Cartoes.EmitirCartoes;
using Application.Interfaces;
using Domain.Entities;
using NSubstitute;

namespace EmissaoCartao.Tests.Application;

public class EmitirCartoesReactionTests
{
    [Fact]
    public async Task ExecuteAsync_DevePersistirCartoesEEnfileirarOutbox()
    {
        // Arrange
        var cartoesRepo = Substitute.For<ICartaoRepository>();
        var outbox = Substitute.For<IOutboxWriter>();
        var sut = new EmitirCartoesReaction(cartoesRepo, outbox);
        var clienteId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();
        var cartoes = new[]
        {
            Cartao.Emitir(clienteId, propostaId, 1, 1000m)
        };

        // Act
        await sut.ExecuteAsync(clienteId, propostaId, cartoes);

        // Assert
        await cartoesRepo.Received(1).AddRangeAsync(cartoes, Arg.Any<CancellationToken>());
        await outbox.Received(1).EnqueueAsync(
            "CartoesEmitidos",
            Arg.Any<string>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());
    }
}
