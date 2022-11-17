using api.Interfaces;
using api.Models.Extract;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExtractController : ControllerBase
{
    private readonly IExtractManager _manager;

    public ExtractController(IExtractManager manager)
    {
        _manager = manager;
    }

    [HttpGet, Route("get/{accountId}")]
    [ProducesResponseType(typeof(api.Models.Movements.Movement), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int accountId, [FromQuery] ExtractViewModel viewModel)
    {
        var movements = await _manager.GetByAccountIdAsync(accountId, viewModel.Index, viewModel.Length);

        if(movements == null || movements.Count < 1)
            return NotFound($"Desculpa, não foram encontradas movimentações para conta {accountId}.");

        return Ok(movements);
    }
    
}