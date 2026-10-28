using Application.Cartoes.ObterCartoes;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/cartoes")]
public sealed class CartoesController : ControllerBase
{
    private readonly ObterCartoesHandler _obter;

    public CartoesController(ObterCartoesHandler obter)
    {
        _obter = obter;
    }

    [HttpGet("{clienteId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<CartaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPorClienteId(
        Guid clienteId,
        CancellationToken cancellationToken)
    {
        var result = await _obter.HandleAsync(clienteId, cancellationToken);
        return Ok(result);
    }
}
