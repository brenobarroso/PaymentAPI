using api.Interfaces;
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

    [HttpGet("get/{accountId}")]
    [ProducesResponseType(typeof(api.Models.Movements.Movement), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(int accountId)
    {
        var movements = await _manager.GetByAccountIdAsync(accountId);

        if(movements == null || movements.Count <= 1)
            return NotFound($"Desculpa, não foram encontradas movimentações para conta {accountId}.");

        return Ok(movements);
    }
    
}