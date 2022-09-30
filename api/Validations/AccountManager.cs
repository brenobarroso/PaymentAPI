using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;

namespace api.Validations;

public class AccountManager : IAccountManager
{
    private readonly PaymentDbContext _context;
    public AccountManager(PaymentDbContext context) => _context = context;

    public async Task<List<Account>> getAllAccountsAsync()
    {
        var result = await _context.Accounts.ToListAsync();
        return result;
    }

    public async Task<Account?> getByCPFAsync(string cpf)
    {
        var result = await _context.Accounts.FindAsync(cpf);

        if(result.Status.ToString() == "Inactive")
            return null;
        if (result == null)
            return null;

        return result;
    }

    public async Task<Account?> getByAccountNumberAsync(string idAccount)
    {
        var result = await _context.Accounts.FindAsync(idAccount);

        if(result.Status.ToString() == "Inactive")
            return null;
        if (result == null)
            return null;

        return result;
    }

    public async Task<Account> CreateAccount(Account person)
    {
        var newAccount = new Account{
            CPF = person.CPF,
            Agency = person.Agency,
            HolderName = person.HolderName,
            Balance = person.Balance
        };

        await _context.Accounts.AddAsync(newAccount);
        await _context.SaveChangesAsync();

        return newAccount;
    }

    // TODO: fazer função de delete e atualizar a interface.
}
