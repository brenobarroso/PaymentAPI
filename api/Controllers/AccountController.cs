using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Models;
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
        return Ok(accounts);
    }

    [HttpGet("get/number/{accountNumber}")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAccount(int accountNumber)
    {
        var account = await _manager.getByAccountNumberAsync(accountNumber);

        if(account == null)
            return NotFound();
        
        return Ok(account);
    }

    [HttpGet("get/cpf/{cpf}")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCPF(string cpf)
    {
        var account = await _manager.getByCPFAsync(cpf);

        if(account == null)
            return NotFound();
        
        return Ok(account);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(Account person)
    {
        var newAccount = new Account{
            CPF = person.CPF,
            Agency = person.Agency,
            HolderName = person.HolderName,
            Balance = person.Balance,
            IsActive = person.IsActive
        };

            var result = await _manager.CreateAccount(newAccount);
            if(result == null)
                UnprocessableEntity("error");
            return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> MakeInactive(int id)
    {
        var account = await _manager.DeleteAccount(id);

        if(account == null)
            return BadRequest();
        
        return Ok(account);
    }
}
