using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Interfaces;
namespace PaymentAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountManager _manager;

    public AccountController(IAccountManager manager)
    {
        _manager = manager;
    }
    

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var accounts = await _manager.getAllAccountsAsync();

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
        var account = await _manager.getByAccountNumberAsync(accountNumber);

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
        var account = await _manager.getByCPFAsync(cpf);

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
}