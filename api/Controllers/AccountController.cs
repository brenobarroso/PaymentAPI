using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Interfaces;
using api.Models.Withdraw;

namespace PaymentAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountManager _manager;
    private readonly IConvertWithdraw _convertWithdraw;
    private readonly IWithdrawManager _managerWithdraw;

    public AccountController(IAccountManager manager, IConvertWithdraw convertWithdraw, IWithdrawManager managerWithdraw)
    {
        _manager = manager;
        _convertWithdraw = convertWithdraw;
        _managerWithdraw = managerWithdraw;
    }
    

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var accounts = await _manager.GetAllAccountsAsync();

        if(accounts == null)
            return NotFound();

        var accountsResult = new List<AccountResult>();

        foreach (Account account in accounts)
        {
            var result = _manager.ConvertToResult(account);
            accountsResult.Add(result);
        }
        return Ok(accountsResult);
    }

    [HttpGet("get/number/{accountNumber}")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAccount(string accountNumber)
    {
        var account = await _manager.GetByAccountNumberAsync(accountNumber);

        if(account == null)
            return NotFound();

        var result = _manager.ConvertToResult(account);
        
        return Ok(result);
    }

    [HttpGet("get/cpf/{cpf}")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCPF(string cpf)
    {
        var account = await _manager.GetByCPFAsync(cpf);
        var withdrawsResult = new List<WithdrawResult>();

        if(account == null)
            return NotFound();

        var result = _manager.ConvertToResult(account);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(AccountViewModel person) // mudar account pra result
    {
        var account = await _manager.CreateAccount(person);
        if(account == null)
            return BadRequest();

        var result = _manager.ConvertToResult(account);
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> MakeInactive(int id) // mudar account pra result
    {
        var account = await _manager.DeleteAccount(id);

        if(account == null)
            return BadRequest();

        var result = _manager.ConvertToResult(account);
        
        return Ok(result);
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