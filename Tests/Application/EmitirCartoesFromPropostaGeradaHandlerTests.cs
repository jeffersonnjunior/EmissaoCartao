using Application.Cartoes.EmitirCartoes;
using Application.Events;
using Application.Interfaces;
using Domain.Entities;
using NSubstitute;

namespace EmissaoCartao.Tests.Application;

public class EmitirCartoesFromPropostaGeradaHandlerTests
{
    private readonly ICartaoRepository _cartoes = Substitute.For<ICartaoRepository>();
    private readonly IProcessedEventStore _processed = Substitute.For<IProcessedEventStore>();
    private readonly IEmitirCartoesReaction _reaction = Substitute.For<IEmitirCartoesReaction>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private EmitirCartoesFromPropostaGeradaHandler CreateSut() =>
        new(_cartoes, _processed, _reaction, _uow);

    private static PropostaGeradaEvent CreateEvent(
        Guid eventId,
        Guid clienteId,
        Guid propostaId,
        string status,
        int quantidade,
        decimal limite) =>
        new(
            eventId,
            "PropostaGerada",
            1,
            DateTime.UtcNow,
            clienteId,
            new PropostaGeradaPayload(propostaId, 300, status, quantidade, limite, limite * quantidade));

    [Fact]
    public async Task HandleAsync_QuandoEventoJaProcessado_NaoReage()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        _processed.ExistsAsync(eventId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await CreateSut().HandleAsync(CreateEvent(eventId, Guid.NewGuid(), Guid.NewGuid(), "Aprovada", 1, 1000m));

        // Assert
        await _reaction.DidNotReceiveWithAnyArgs().ExecuteAsync(default, default!, default!, default);
        await _uow.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task HandleAsync_QuandoNegada_MarcaProcessadoSemEmitir()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();
        _processed.ExistsAsync(eventId, Arg.Any<CancellationToken>()).Returns(false);
        _cartoes.ExistsByPropostaIdAsync(propostaId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await CreateSut().HandleAsync(CreateEvent(eventId, Guid.NewGuid(), propostaId, "Negada", 0, 0m));

        // Assert
        await _reaction.DidNotReceiveWithAnyArgs().ExecuteAsync(default, default!, default!, default);
        await _processed.Received(1).MarkProcessedAsync(eventId, "PropostaGerada", Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_QuandoAprovadaComUmCartao_EmiteUm()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();
        _processed.ExistsAsync(eventId, Arg.Any<CancellationToken>()).Returns(false);
        _cartoes.ExistsByPropostaIdAsync(propostaId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await CreateSut().HandleAsync(CreateEvent(eventId, clienteId, propostaId, "Aprovada", 1, 1000m));

        // Assert
        await _reaction.Received(1).ExecuteAsync(
            clienteId,
            propostaId,
            Arg.Is<IReadOnlyList<Cartao>>(c => c.Count == 1 && c[0].Limite == 1000m && c[0].Sequencia == 1),
            Arg.Any<CancellationToken>());
        await _processed.Received(1).MarkProcessedAsync(eventId, "PropostaGerada", Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_QuandoAprovadaComDoisCartoes_EmiteDois()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();
        _processed.ExistsAsync(eventId, Arg.Any<CancellationToken>()).Returns(false);
        _cartoes.ExistsByPropostaIdAsync(propostaId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await CreateSut().HandleAsync(CreateEvent(eventId, clienteId, propostaId, "Aprovada", 2, 5000m));

        // Assert
        await _reaction.Received(1).ExecuteAsync(
            clienteId,
            propostaId,
            Arg.Is<IReadOnlyList<Cartao>>(c =>
                c.Count == 2 &&
                c.All(x => x.Limite == 5000m) &&
                c.Select(x => x.Sequencia).OrderBy(x => x).SequenceEqual(new[] { 1, 2 })),
            Arg.Any<CancellationToken>());
        await _processed.Received(1).MarkProcessedAsync(eventId, "PropostaGerada", Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_QuandoPropostaJaTemCartoes_DeveSerIdempotente()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();
        _processed.ExistsAsync(eventId, Arg.Any<CancellationToken>()).Returns(false);
        _cartoes.ExistsByPropostaIdAsync(propostaId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await CreateSut().HandleAsync(CreateEvent(eventId, Guid.NewGuid(), propostaId, "Aprovada", 1, 1000m));

        // Assert
        await _reaction.DidNotReceiveWithAnyArgs().ExecuteAsync(default, default!, default!, default);
        await _processed.Received(1).MarkProcessedAsync(eventId, "PropostaGerada", Arg.Any<CancellationToken>());
    }
}
