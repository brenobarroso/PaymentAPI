using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovementController : ControllerBase
{
    private readonly IExtractManager _manager;

    public MovementController(IExtractManager manager)
    {
        _manager = manager;
    }

    [HttpGet("get/{accountId}")]
    [ProducesResponseType(typeof(api.Models.Movements.Movement), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(int accountId)
    {
        var movements = await _manager.GetByAccountIdAsync(accountId);

        if(movements == null || movements.Count == 0)
            return NotFound("Sorry, was not found any movement for this Id!");

        return Ok(movements);
    }
    
}