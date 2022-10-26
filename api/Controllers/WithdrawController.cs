using api.Interfaces;
using api.Models.Withdraws;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WithdrawController : ControllerBase
{
    private readonly IConvertWithdraw _convertWithdraw;
    private readonly IWithdrawManager _managerWithdraw;

    public WithdrawController(IConvertWithdraw convertWithdraw, IWithdrawManager managerWithdraw)
    {
        _convertWithdraw = convertWithdraw;
        _managerWithdraw = managerWithdraw;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var withdraws = await _managerWithdraw.GetAllWithdrawsAsync();

        if(withdraws == null)
            return NotFound("Sorry, there is no withdraws!");
        
        return Ok(withdraws);
    }

    [HttpGet("get/{accountId}")]
    [ProducesResponseType(typeof(Withdraw), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int accountId)
    {
        var withdraw = await _managerWithdraw.GetWithdrawsByIdAsync(accountId);

        if(withdraw == null || withdraw.Count == 0)
            return NotFound("Sorry, was not found any withdraw for this Id!");

        return Ok(withdraw);
    }

    [HttpPut]
    public async Task<IActionResult> MakeAWithdraw(string accountNumber, decimal value)
    {
        var query = await _managerWithdraw.MakeWithdraw(accountNumber, value);

        if(query.account == null)
            return BadRequest();

        var result = _convertWithdraw.ConvertToResultWithdraw(query.account);
        
        if(query.sucess)
            return Ok(result);
        
        return UnprocessableEntity("withdraw reproved");
    }
}
